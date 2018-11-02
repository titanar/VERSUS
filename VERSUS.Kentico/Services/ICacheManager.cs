using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VERSUS.Kentico.Areas.WebHooks.Models;

namespace VERSUS.Kentico.Services
{
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// Gets an existing cache entry or creates one using the supplied <paramref name="valueFactory"/>.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry value.</typeparam>
        /// <param name="identifierTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="valueFactory">Method to create the entry.</param>
        /// <param name="skipCacheDelegate">Method to check whether a cache entry should be created (TRUE to skip creation of the entry).</param>
        /// <param name="dependencyListFactory">Method to get a collection of identifiers of entries that the current entry depends upon.</param>
        /// <param name="createCacheEntriesInBackground">Flag saying if cache entry should be off-loaded to a background thread.</param>
        /// <returns>The cache entry value, either cached or obtained through the <paramref name="valueFactory"/>.</returns>
        Task<T> GetOrCreateAsync<T>(IEnumerable<string> identifierTokens, Func<Task<T>> valueFactory, Func<T, bool> skipCacheDelegate, Func<T, IEnumerable<CacheTokenPair>> dependencyListFactory);

        /// <summary>
        /// Tries to get a cache entry.
        /// </summary>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <param name="identifierTokens">String tokens that form a unique identifier of the entry.</param>
        /// <param name="value">The cache entry value, if it exists.</param>
        /// <returns>True if the entry exists, otherwise false.</returns>
        bool TryGetValue<T>(IEnumerable<string> identifierTokens, out T value)
            where T : class;

        /// <summary>
        /// Invalidates (clears) a cache entry.
        /// </summary>
        /// <param name="identifiers">Identifiers of the entry.</param>
        void InvalidateEntry(CacheTokenPair identifiers);

        /// <summary>
        /// Looks up the cache for an entry and passes it to <paramref name="dependencyFactory"/> that extracts specific dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the cache entry.</typeparam>
        /// <param name="cacheIdentifierPair">Identifiers used to look up the cache for the entry.</param>
        /// <param name="dependencyFactory">The method that takes the entry, and uses them to extract dependencies from it.</param>
        /// <returns>Identifiers of the dependencies.</returns>
        IEnumerable<CacheTokenPair> GetDependenciesFromCache<T>(CacheTokenPair identifierSet, Func<T, IEnumerable<CacheTokenPair>> dependencyListFactory)
            where T : class;
    }
}