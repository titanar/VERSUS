using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using VERSUS.Core;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Services.Models;

namespace VERSUS.Kentico.Services
{
    public class CacheManager : ICacheManager
    {
        #region Constants

        private const string DUMMY_IDENTIFIER = "dummy";

        #endregion Constants

        #region Fields

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<string, object> _cacheDummyLocks = new ConcurrentDictionary<string, object>();
        private readonly object _entryCreationLock = new object();
        private readonly bool _disposed;
        private readonly int _cacheExpirySeconds;
        private readonly bool _createCacheEntriesInBackground;
        private readonly IMemoryCache _memoryCache;

        #endregion Fields

        #region Constructors

        public CacheManager(IOptionsSnapshot<VersusOptions> versusOptions, IMemoryCache memoryCache)
        {
            _cacheExpirySeconds = versusOptions.Value.CacheTimeoutSeconds;
            _createCacheEntriesInBackground = versusOptions.Value.CreateCacheEntriesInBackground;
            _memoryCache = memoryCache;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Gets an existing cache entry or creates one using the supplied <paramref name="valueFactory"/>.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry value.</typeparam>
        /// <param name="keyTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="valueFactory">Method to create the entry.</param>
        /// <param name="skipCacheDelegate">Method to check whether a cache entry should be created (TRUE to skip creation of the entry).</param>
        /// <param name="dependencyFactory">Method to get a collection of identifiers of entries that the current entry depends upon.</param>
        /// <param name="createCacheEntriesInBackground">Flag saying if cache entry should be off-loaded to a background thread.</param>
        /// <returns>The cache entry value, either cached or obtained through the <paramref name="valueFactory"/>.</returns>
        public async Task<T> GetOrCreateAsync<T>(IEnumerable<string> keyTokens, Func<Task<T>> valueFactory, Func<Task<T>> previewValueFactory, Func<T, bool> skipCacheDelegate, Func<T, IEnumerable<CacheTokenPair>> dependencyFactory)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var key = string.Join("|", keyTokens);

                if (!_memoryCache.TryGetValue(key, out T entry))
                {
                    // If it doesn't exist, get it via valueFactory
                    T value = await valueFactory();

                    if (!skipCacheDelegate(value))
                    {
                        // Create it in a background thread.
                        if (_createCacheEntriesInBackground)
                        {
                            _ = Task.Run(() => CreateEntry(key, value, previewValueFactory, dependencyFactory));
                        }
                        else
                        {
                            CreateEntry(key, value, previewValueFactory, dependencyFactory);
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
        /// Tries to get a cache entry.
        /// </summary>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <param name="keyTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="value">The cache entry value, if it exists.</param>
        /// <returns>True if the entry exists, otherwise false.</returns>
        public bool TryGetValue<T>(IEnumerable<string> keyTokens, out T value)
            where T : class
        {
            return _memoryCache.TryGetValue(string.Join("|", keyTokens), out value);
        }

        /// <summary>
        /// Invalidates (clears) a cache entry.
        /// </summary>
        /// <param name="cacheTokenPair">Identifiers of the entry.</param>
        public void InvalidateEntry(string typeName, string codename)
        {
            foreach (var dependentTypeName in KenticoCloudCacheHelper.GetDependentTypeNames(typeName))
            {
                if (_memoryCache.TryGetValue(string.Join("|", DUMMY_IDENTIFIER, dependentTypeName, codename), out CancellationTokenSource dummyEntry))
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
        public IEnumerable<CacheTokenPair> GetDependenciesFromCache<T>(CacheTokenPair cacheIdentifierPair, Func<T, IEnumerable<CacheTokenPair>> dependencyFactory)
            where T : class
        {
            if (TryGetValue(new[] { cacheIdentifierPair.TypeName, cacheIdentifierPair.Codename }, out T cacheEntry))
            {
                return dependencyFactory(cacheEntry);
            }

            return null;
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Creates a new cache entry.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry value.</typeparam>
        /// <param name="key">String tokens that form a unique identifier of the entry.</param>
        /// <param name="value">Value of the entry.</param>
        /// <param name="dependencyFactory">Method to get a collection of identifier of entries that the current entry depends upon.</param>
        private async void CreateEntry<T>(string key, T value, Func<Task<T>> previewValueFactory, Func<T, IEnumerable<CacheTokenPair>> dependencyFactory)
        {
            T dependencyValue = previewValueFactory != null ? await previewValueFactory() : value;
            var dependencies = dependencyFactory(dependencyValue) ?? new List<CacheTokenPair>();

            // Restart entries' expiration period each time they're requested.
            var entryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheExpirySeconds));

            foreach (var dependency in dependencies)
            {
                var dummyKeyTokens = new[] { DUMMY_IDENTIFIER, dependency.TypeName, dependency.Codename };
                var dummyKey = string.Join("|", dummyKeyTokens);
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
                        _memoryCache.Set(key, value, entryOptions);
                    }
                }
            }
        }

        private bool EntryExists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        private CancellationTokenSource GetOrCreateDummyEntry(string dummyKey)
        {
            if (!DummyEntryExists(dummyKey, out CancellationTokenSource dummyEntry))
            {
                dummyEntry = _memoryCache.Set(dummyKey, new CancellationTokenSource(), new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove });
            }

            return dummyEntry;
        }

        private bool DummyEntryExists(string dummyKey, out CancellationTokenSource dummyEntry)
        {
            return _memoryCache.TryGetValue(dummyKey, out dummyEntry) && !dummyEntry.IsCancellationRequested;
        }

        #endregion Private methods
    }
}