using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Versus.Core.Extensions;
using VERSUS.Core;
using VERSUS.Kentico.Areas.WebHooks.Models;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Resolvers;

namespace VERSUS.Kentico.Services
{
    public class ReactiveCacheManager : ICacheManager
    {
        #region Fields

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly IDependentTypesResolver _dependentTypesResolver;
        private readonly ConcurrentDictionary<string, object> _cacheDummyLocks = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, object> _cacheLocks = new ConcurrentDictionary<string, object>();
        private readonly object _dummyEntryCreationLock = new object();
        private readonly object _entryCreationLock = new object();
        private bool _disposed;

        #endregion

        #region Properties

        public int CacheExpirySeconds { get; }

        IMemoryCache MemoryCache { get; }

        protected List<string> InvalidatingOperations => new List<string>
        {
            "upsert",
            "publish",
            "restore_publish",
            "unpublish",
            "archive",
            "restore"
        };

        #endregion

        #region Constructors

        public ReactiveCacheManager(IOptions<VersusOptions> options, IMemoryCache memoryCache, IDependentTypesResolver dependentTypesResolver, IWebhookListener webhookListener)
        {
            CacheExpirySeconds = options.Value.CacheTimeoutSeconds;
            MemoryCache = memoryCache;
            _dependentTypesResolver = dependentTypesResolver;

            WebhookObservableFactory
                .GetObservable(webhookListener, nameof(webhookListener.WebhookNotification))
                .Where(args => InvalidatingOperations.Any(operation => operation.Equals(args.Operation, StringComparison.Ordinal)))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(args => InvalidateEntry(args.IdentifierSet));
        }

        #endregion

        #region "Public methods"

        /// <summary>
        /// Gets an existing cache entry or creates one using the supplied <paramref name="valueFactory"/>.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry value.</typeparam>
        /// <param name="identifierTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="valueFactory">Method to create the entry.</param>
        /// <param name="skipCacheDelegate">Method to check whether a cache entry should be created (TRUE to skip creation of the entry).</param>
        /// <param name="dependencyFactory">Method to get a collection of identifiers of entries that the current entry depends upon.</param>
        /// <param name="createCacheEntriesInBackground">Flag saying if cache entry should be off-loaded to a background thread.</param>
        /// <returns>The cache entry value, either cached or obtained through the <paramref name="valueFactory"/>.</returns>
        public async Task<T> GetOrCreateAsync<T>(IEnumerable<string> identifierTokens, Func<Task<T>> valueFactory, Func<T, bool> skipCacheDelegate, Func<T, IEnumerable<CacheIdentifierPair>> dependencyFactory, bool createCacheEntriesInBackground = false)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var key = string.Join("|", identifierTokens);

                if (!MemoryCache.TryGetValue(key, out T entry))
                {
                    // If it doesn't exist, get it via valueFactory
                    T value = await valueFactory();

                    if (!skipCacheDelegate(value))
                    {
                        // Create it in a background thread.
                        if (createCacheEntriesInBackground)
                        {
                            _ = Task.Run(() => CreateEntry(key, value, dependencyFactory));
                        }
                        else
                        {
                            CreateEntry(key, value, dependencyFactory);
                        }
                    }

                    return value;
                }

                return entry;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Creates a new cache entry.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry value.</typeparam>
        /// <param name="key">String tokens that form a unique identifier of the entry.</param>
        /// <param name="value">Value of the entry.</param>
        /// <param name="dependencyFactory">Method to get a collection of identifier of entries that the current entry depends upon.</param>
        private void CreateEntry<T>(string key, T value, Func<T, IEnumerable<CacheIdentifierPair>> dependencyFactory)
        {
            var dependencies = dependencyFactory(value) ?? new List<CacheIdentifierPair>();

            // Restart entries' expiration period each time they're requested.
            var entryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(CacheExpirySeconds)
            };

            foreach (var dependency in dependencies)
            {
                var dummyIdentifierTokens = new List<string> { KenticoCloudCacheHelper.DUMMY_IDENTIFIER, dependency.TypeName, dependency.Codename };
                var dummyKey = string.Join("|", dummyIdentifierTokens);
                var newDummyLock = new object();
                object dummyLock;

                if (_cacheDummyLocks.TryAdd(dummyKey, newDummyLock))
                {
                    dummyLock = newDummyLock;
                }
                else
                {
                    dummyLock = _cacheDummyLocks[dummyKey];
                }

                // Dummy entries hold just the CancellationTokenSource
                if (!DummyEntryExists(dummyKey, out CancellationTokenSource dummyEntry))
                {
                    lock (dummyLock)
                    {
                        dummyEntry = GetOrCreateDummyEntry(dummyKey);
                    }
                }

                // Subscribe the main entry to dummy entry's cancellation token
                entryOptions.AddExpirationToken(new CancellationChangeToken(dummyEntry.Token));
            }

            if (!EntryExists(key))
            {
                lock (_entryCreationLock)
                {
                    if (!EntryExists(key))
                    {
                        MemoryCache.Set(key, value, entryOptions);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to get a cache entry.
        /// </summary>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <param name="identifierTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="value">The cache entry value, if it exists.</param>
        /// <returns>True if the entry exists, otherwise false.</returns>
        public bool TryGetValue<T>(IEnumerable<string> identifierTokens, out T value)
            where T : class
        {
            return MemoryCache.TryGetValue(string.Join("|", identifierTokens), out value);
        }

        /// <summary>
        /// Invalidates (clears) a cache entry.
        /// </summary>
        /// <param name="dependency">Identifiers of the entry.</param>
        public void InvalidateEntry(CacheIdentifierPair dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            foreach (var dependentTypeName in _dependentTypesResolver.GetDependentTypeNames(dependency.TypeName))
            {
                if (MemoryCache.TryGetValue(string.Join("|", KenticoCloudCacheHelper.DUMMY_IDENTIFIER, dependentTypeName, dependency.Codename), out CancellationTokenSource dummyEntry))
                {
                    // Mark all subscribers to the CancellationTokenSource as invalid.
                    dummyEntry.Cancel();
                }
            }
        }

        /// <summary>
        /// Looks up the cache for an entry and passes it to <paramref name="dependencyFactory"/> that extracts specific dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="cacheIdentifierPair">Identifiers used to look up the cache for the entry.</param>
        /// <param name="dependencyFactory">The method that takes the entry, and uses them to extract dependencies from it.</param>
        /// <returns>Identifiers of the dependencies.</returns>
        public IEnumerable<CacheIdentifierPair> GetDependenciesFromCache<T>(CacheIdentifierPair cacheIdentifierPair, Func<T, IEnumerable<CacheIdentifierPair>> dependencyFactory)
            where T : class
        {
            if (MemoryCache.TryGetValue(string.Join("|", cacheIdentifierPair.TypeName, cacheIdentifierPair.Codename ), out T cacheEntry))
            {
                return dependencyFactory(cacheEntry);
            }

            return null;
        }

        /// <summary>
        /// Gets dependent identifier pairs and passes them to <paramref name="dependencyFactory"/> that extracts specific dependencies.
        /// </summary>
        /// <param name="typeName">The original type of the cache entry.</param>
        /// <param name="codename">The code name of the cache entry.</param>
        /// <param name="dependencyListFactory">The method that takes each of the identifiers of the dependent types (formats), and uses them to extract dependencies.</param>
        /// <returns>Identifiers of the dependencies.</returns>
        public IEnumerable<CacheIdentifierPair> GetDependentIdentifierPairs(string typeName, string codename, Func<CacheIdentifierPair, IEnumerable<CacheIdentifierPair>> dependencyFactory)
        {
            foreach (var dependentTypeName in _dependentTypesResolver.GetDependentTypeNames(typeName))
            {
                return dependencyFactory(new CacheIdentifierPair { TypeName = dependentTypeName, Codename = codename });
            }

            return null;
        }

        /// <summary>
        /// The <see cref="IDisposable.Dispose"/> implementation.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Non-public methods"

        protected bool EntryExists(string key)
        {
            return MemoryCache.TryGetValue(key, out _);
        }

        protected CancellationTokenSource GetOrCreateDummyEntry(string dummyKey)
        {
            if (!DummyEntryExists(dummyKey, out CancellationTokenSource dummyEntry))
            {
                dummyEntry = MemoryCache.Set(dummyKey, new CancellationTokenSource(), new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove });
            }

            return dummyEntry;
        }

        protected bool DummyEntryExists(string dummyKey, out CancellationTokenSource dummyEntry)
        {
            return MemoryCache.TryGetValue(dummyKey, out dummyEntry) && !dummyEntry.IsCancellationRequested;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                MemoryCache.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}