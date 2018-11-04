using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KenticoCloud.Delivery;
using KenticoCloud.Delivery.InlineContentItems;
using Newtonsoft.Json.Linq;
using VERSUS.Core.Extensions;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Webhooks.Models;

namespace VERSUS.Kentico.Services
{
    public class CachedDeliveryClient : IDeliveryClient
    {
        private ICacheManager _cacheManager;
        private IDeliveryClient _deliveryClient;

        #region Properties

        public IContentLinkUrlResolver ContentLinkUrlResolver
        {
            get => _deliveryClient.ContentLinkUrlResolver;
            set => _deliveryClient.ContentLinkUrlResolver = value;
        }

        public ICodeFirstModelProvider CodeFirstModelProvider
        {
            get => _deliveryClient.CodeFirstModelProvider;
            set => _deliveryClient.CodeFirstModelProvider = value;
        }

        public IInlineContentItemsProcessor InlineContentItemsProcessor => _deliveryClient.InlineContentItemsProcessor;

        #endregion Properties

        #region Constructors

        public CachedDeliveryClient(ICacheManager cacheManager, IDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
            _cacheManager = cacheManager;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Returns a content item as JSON data.
        /// </summary>
        /// <param name="codename">The codename of a content item.</param>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for projection or depth of modular content.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content item with the specified codename.</returns>
        public async Task<JObject> GetItemJsonAsync(string codename, params string[] parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_JSON_IDENTIFIER, codename };
            identifierTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemJsonAsync(codename, parameters),
                response => response == null,
                GetContentItemSingleJsonDependencies);
        }

        /// <summary>
        /// Returns content items as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for filtering, ordering or depth of modular content.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content items. If no query parameters are specified, all content items are returned.</returns>
        public async Task<JObject> GetItemsJsonAsync(params string[] parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_JSON_IDENTIFIER };
            identifierTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemsJsonAsync(parameters),
                response => response["items"].Count() <= 0,
                GetContentItemListingJsonDependencies);
        }

        /// <summary>
        /// Returns a content item.
        /// </summary>
        /// <param name="codename">The codename of a content item.</param>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for projection or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemResponse"/> instance that contains the content item with the specified codename.</returns>
        public async Task<DeliveryItemResponse> GetItemAsync(string codename, params IQueryParameter[] parameters)
        {
            return await GetItemAsync(codename, (IEnumerable<IQueryParameter>)parameters);
        }

        /// <summary>
        /// Gets one strongly typed content item by its codename.
        /// </summary>
        /// <typeparam name="T">Type of the code-first model. (Or <see cref="object"/> if the return type is not yet known.)</typeparam>
        /// <param name="codename">The codename of a content item.</param>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for projection or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemResponse{T}"/> instance that contains the content item with the specified codename.</returns>
        public async Task<DeliveryItemResponse<T>> GetItemAsync<T>(string codename, params IQueryParameter[] parameters)
        {
            return await GetItemAsync<T>(codename, (IEnumerable<IQueryParameter>)parameters);
        }

        /// <summary>
        /// Returns a content item.
        /// </summary>
        /// <param name="codename">The codename of a content item.</param>
        /// <param name="parameters">A collection of query parameters, for example for projection or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemResponse"/> instance that contains the content item with the specified codename.</returns>
        public async Task<DeliveryItemResponse> GetItemAsync(string codename, IEnumerable<IQueryParameter> parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER, codename };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemAsync(codename, parameters),
                response => response == null,
                GetContentItemSingleDependencies);
        }

        /// <summary>
        /// Gets one strongly typed content item by its codename.
        /// </summary>
        /// <typeparam name="T">Type of the code-first model. (Or <see cref="object"/> if the return type is not yet known.)</typeparam>
        /// <param name="codename">The codename of a content item.</param>
        /// <param name="parameters">A collection of query parameters, for example for projection or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemResponse{T}"/> instance that contains the content item with the specified codename.</returns>
        public async Task<DeliveryItemResponse<T>> GetItemAsync<T>(string codename, IEnumerable<IQueryParameter> parameters = null)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_TYPED_IDENTIFIER, codename };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemAsync<T>(codename, parameters),
                response => response == null,
                GetContentItemSingleDependencies);
        }

        /// <summary>
        /// Searches the content repository for items that match the filter criteria.
        /// Returns content items.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for filtering, ordering or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemListingResponse"/> instance that contains the content items. If no query parameters are specified, all content items are returned.</returns>
        public async Task<DeliveryItemListingResponse> GetItemsAsync(params IQueryParameter[] parameters)
        {
            return await GetItemsAsync((IEnumerable<IQueryParameter>)parameters);
        }

        /// <summary>
        /// Returns content items.
        /// </summary>
        /// <param name="parameters">A collection of query parameters, for example for filtering, ordering or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemListingResponse"/> instance that contains the content items. If no query parameters are specified, all content items are returned.</returns>
        public async Task<DeliveryItemListingResponse> GetItemsAsync(IEnumerable<IQueryParameter> parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_IDENTIFIER };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemsAsync(parameters),
                response => response.Items.Count <= 0,
                GetContentItemListingDependencies);
        }

        /// <summary>
        /// Searches the content repository for items that match the filter criteria.
        /// Returns strongly typed content items.
        /// </summary>
        /// <typeparam name="T">Type of the code-first model. (Or <see cref="object"/> if the return type is not yet known.)</typeparam>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for filtering, ordering or depth of modular content.</param>
        /// <returns>The <see cref="DeliveryItemListingResponse{T}"/> instance that contains the content items. If no query parameters are specified, all content items are returned.</returns>
        public async Task<DeliveryItemListingResponse<T>> GetItemsAsync<T>(params IQueryParameter[] parameters)
        {
            return await GetItemsAsync<T>((IEnumerable<IQueryParameter>)parameters);
        }

        public async Task<DeliveryItemListingResponse<T>> GetItemsAsync<T>(IEnumerable<IQueryParameter> parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_TYPED_IDENTIFIER };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetItemsAsync<T>(parameters),
                response => response.Items.Count <= 0,
                GetContentItemListingDependencies);
        }

        /// <summary>
        /// Returns a content type as JSON data.
        /// </summary>
        /// <param name="codename">The codename of a content type.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content type with the specified codename.</returns>
        public async Task<JObject> GetTypeJsonAsync(string codename)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_JSON_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTypeJsonAsync(codename),
                response => response == null,
                GetTypeSingleJsonDependencies);
        }

        /// <summary>
        /// Returns content types as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for paging.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content types. If no query parameters are specified, all content types are returned.</returns>
        public async Task<JObject> GetTypesJsonAsync(params string[] parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_LISTING_JSON_IDENTIFIER };
            identifierTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTypesJsonAsync(parameters),
                response => response["types"].Count() <= 0,
                GetTypeListingJsonDependencies);
        }

        /// <summary>
        /// Returns a content type.
        /// </summary>
        /// <param name="codename">The codename of a content type.</param>
        /// <returns>The content type with the specified codename.</returns>
        public async Task<ContentType> GetTypeAsync(string codename)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTypeAsync(codename),
                response => response == null,
                GetTypeSingleDependencies);
        }

        /// <summary>
        /// Returns content types.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for paging.</param>
        /// <returns>The <see cref="DeliveryTypeListingResponse"/> instance that represents the content types. If no query parameters are specified, all content types are returned.</returns>
        public async Task<DeliveryTypeListingResponse> GetTypesAsync(params IQueryParameter[] parameters)
        {
            return await GetTypesAsync((IEnumerable<IQueryParameter>)parameters);
        }

        /// <summary>
        /// Returns content types.
        /// </summary>
        /// <param name="parameters">A collection of query parameters, for example for paging.</param>
        /// <returns>The <see cref="DeliveryTypeListingResponse"/> instance that represents the content types. If no query parameters are specified, all content types are returned.</returns>
        public async Task<DeliveryTypeListingResponse> GetTypesAsync(IEnumerable<IQueryParameter> parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_LISTING_IDENTIFIER };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTypesAsync(parameters),
                response => response.Types.Count <= 0,
                GetTypeListingDependencies);
        }

        /// <summary>
        /// Returns a content element.
        /// </summary>
        /// <param name="contentTypeCodename">The codename of the content type.</param>
        /// <param name="contentElementCodename">The codename of the content element.</param>
        /// <returns>A content element with the specified codename that is a part of a content type with the specified codename.</returns>
        public async Task<ContentElement> GetContentElementAsync(string contentTypeCodename, string contentElementCodename)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ELEMENT_IDENTIFIER, contentTypeCodename, contentElementCodename };

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetContentElementAsync(contentTypeCodename, contentElementCodename),
                response => response == null,
                GetContentElementDependencies);
        }

        /// <summary>
        /// Returns a taxonomy group as JSON data.
        /// </summary>
        /// <param name="codename">The codename of a taxonomy group.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the taxonomy group with the specified codename.</returns>
        public async Task<JObject> GetTaxonomyJsonAsync(string codename)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTaxonomyJsonAsync(codename),
                response => response == null,
                GetTaxonomySingleJsonDependency);
        }

        /// <summary>
        /// Returns taxonomy groups as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example, for paging.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the taxonomy groups. If no query parameters are specified, all taxonomy groups are returned.</returns>
        public async Task<JObject> GetTaxonomiesJsonAsync(params string[] parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_LISTING_JSON_IDENTIFIER };
            identifierTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTaxonomiesJsonAsync(parameters),
                response => response["taxonomies"].Count() <= 0,
                GetTaxonomyListingJsonDependencies);
        }

        /// <summary>
        /// Returns a taxonomy group.
        /// </summary>
        /// <param name="codename">The codename of a taxonomy group.</param>
        /// <returns>The taxonomy group with the specified codename.</returns>
        public async Task<TaxonomyGroup> GetTaxonomyAsync(string codename)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTaxonomyAsync(codename),
                response => response == null,
                GetTaxonomySingleDependency);
        }

        /// <summary>
        /// Returns taxonomy groups.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example, for paging.</param>
        /// <returns>The <see cref="DeliveryTaxonomyListingResponse"/> instance that represents the taxonomy groups. If no query parameters are specified, all taxonomy groups are returned.</returns>
        public async Task<DeliveryTaxonomyListingResponse> GetTaxonomiesAsync(params IQueryParameter[] parameters)
        {
            return await GetTaxonomiesAsync((IEnumerable<IQueryParameter>)parameters);
        }

        /// <summary>
        /// Returns taxonomy groups.
        /// </summary>
        /// <param name="parameters">A collection of query parameters, for example, for paging.</param>
        /// <returns>The <see cref="DeliveryTaxonomyListingResponse"/> instance that represents the taxonomy groups. If no query parameters are specified, all taxonomy groups are returned.</returns>
        public async Task<DeliveryTaxonomyListingResponse> GetTaxonomiesAsync(IEnumerable<IQueryParameter> parameters)
        {
            var identifierTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_LISTING_IDENTIFIER };
            identifierTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                identifierTokens,
                () => _deliveryClient.GetTaxonomiesAsync(parameters),
                response => response.Taxonomies.Count <= 0,
                GetTaxonomyListingDependencies);
        }

        #endregion Public methods

        #region Dependency resolvers

        /// <summary>
        /// Extracts identifier sets from a single-item response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryItemResponse"/> or <see cref="DeliveryItemResponse{T}"/>, either strongly-typed, or runtime-typed.</param>
        /// <returns>Identifiers of all formats of the item, its modular content items, taxonomies used in elements, underlying content type and eventually its elements (when present in the cache).</returns>
        private IEnumerable<CacheTokenPair> GetContentItemSingleDependencies(dynamic response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemSingleResponse(response) && response?.Item != null)
            {
                dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentItemDependencies(response.Item));

                foreach (var item in response.LinkedItems)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single-item JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> response.</param>
        /// <returns>Identifiers of all formats of the item, its modular content items, taxonomies used in elements, underlying content type and eventually its elements (when present in the cache).</returns>
        private IEnumerable<CacheTokenPair> GetContentItemSingleJsonDependencies(JObject response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemSingleJsonResponse(response))
            {
                dependencies.AddNonNullRange(GetContentItemDependencies(response[KenticoCloudCacheHelper.ITEM_IDENTIFIER]));

                foreach (var item in response[KenticoCloudCacheHelper.MODULAR_CONTENT_IDENTIFIER]?.Children())
                {
                    dependencies.AddNonNullRange(GetContentItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from an item listing response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryItemListingResponse"/> or <see cref="DeliveryItemListingResponse{T}"/>, either strongly-typed, or runtime-typed.</param>
        /// <returns>Identifiers of all formats of the items, their modular content items, taxonomies used in elements, underlying content types and eventually their elements (when present in the cache).</returns>
        private IEnumerable<CacheTokenPair> GetContentItemListingDependencies(dynamic response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemListingResponse(response))
            {
                foreach (dynamic item in response.Items)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentItemDependencies(item));
                }

                foreach (var item in response.LinkedItems)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a item listing JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> item listing response.</param>
        /// <returns>Identifiers of all formats of the items, their modular content items, taxonomies used in elements, underlying content types and eventually their elements (when present in the cache).</returns>
        private IEnumerable<CacheTokenPair> GetContentItemListingJsonDependencies(JObject response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemListingJsonResponse(response))
            {
                foreach (dynamic item in response[KenticoCloudCacheHelper.ITEMS_IDENTIFIER].Children())
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentItemDependencies(item));
                }

                foreach (var item in response[KenticoCloudCacheHelper.MODULAR_CONTENT_IDENTIFIER]?.Children())
                {
                    dependencies.AddNonNullRange(GetContentItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content element response.
        /// </summary>
        /// <param name="response">The <see cref="ContentElement"/> response.</param>
        /// <returns>Identifiers of all formats of the element.</returns>
        private IEnumerable<CacheTokenPair> GetContentElementDependencies(ContentElement response)
        {
            return GetContentElementDependenciesInternal(KenticoCloudCacheHelper.CONTENT_ELEMENT_IDENTIFIER, response).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single taxonomy group response.
        /// </summary>
        /// <param name="response">The <see cref="TaxonomyGroup"/> response.</param>
        /// <returns>Identifiers of all formats of the taxonomy.</returns>
        private IEnumerable<CacheTokenPair> GetTaxonomySingleDependency(TaxonomyGroup response)
        {
            return GetTaxonomyDependencies(KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, response?.System?.Codename).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single taxonomy group JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> taxonomy response.</param>
        /// <returns>Identifiers of all formats of the taxonomy.</returns>
        private IEnumerable<CacheTokenPair> GetTaxonomySingleJsonDependency(JObject response)
        {
            return GetTaxonomyDependencies(
                KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER,
                response?[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.CODENAME_IDENTIFIER]?.ToString()).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a listing taxonomy group response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryTaxonomyListingResponse"/> response.</param>
        /// <returns>Identifiers of all formats of all the taxonomies.</returns>
        private IEnumerable<CacheTokenPair> GetTaxonomyListingDependencies(DeliveryTaxonomyListingResponse response)
        {
            return response?.Taxonomies?.SelectMany(t => GetTaxonomySingleDependency(t)).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a listing taxonomy group JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> response.</param>
        /// <returns>Identifiers of all formats of all the taxonomies.</returns>
        private IEnumerable<CacheTokenPair> GetTaxonomyListingJsonDependencies(JObject response)
        {
            return response?[KenticoCloudCacheHelper.TAXONOMIES_IDENTIFIER]?.SelectMany(t => GetTaxonomySingleJsonDependency(t.ToObject<JObject>())).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type response.
        /// </summary>
        /// <param name="response">The <see cref="ContentType"/> response.</param>
        /// <returns>Identifiers of all formats of the content type and its elements.</returns>
        private IEnumerable<CacheTokenPair> GetTypeSingleDependencies(ContentType response)
        {
            return GetContentTypeDependencies(KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, response?.System?.Codename, response).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> content type response.</param>
        /// <returns>Identifiers of all formats of the content type and its elements.</returns>
        private IEnumerable<CacheTokenPair> GetTypeSingleJsonDependencies(JObject response)
        {
            return GetContentTypeDependencies(
                KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_JSON_IDENTIFIER,
                response?[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.CODENAME_IDENTIFIER]?.ToString(), response).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type listing response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryTypeListingResponse"/> response.</param>
        /// <returns>Identifiers of all formats of all the content types and their elements.</returns>
        private IEnumerable<CacheTokenPair> GetTypeListingDependencies(DeliveryTypeListingResponse response)
        {
            return response?.Types?.SelectMany(t => GetTypeSingleDependencies(t)).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type listing JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> content type listing response.</param>
        /// <returns>Identifiers of all formats of all the content types and their elements.</returns>
        private IEnumerable<CacheTokenPair> GetTypeListingJsonDependencies(JObject response)
        {
            return response?[KenticoCloudCacheHelper.TYPES_IDENTIFIER]?.SelectMany(t => GetTypeSingleJsonDependencies(t.ToObject<JObject>())).Distinct();
        }

        private IEnumerable<CacheTokenPair> GetDependentIdentifierPairs(string typeName, string codename, Func<CacheTokenPair, IEnumerable<CacheTokenPair>> dependencyFactory)
        {
            foreach (var dependentTypeName in KenticoCloudCacheHelper.GetDependentTypeNames(typeName))
            {
                return dependencyFactory(new CacheTokenPair { TypeName = dependentTypeName, Codename = codename });
            }

            return null;
        }

        private IEnumerable<CacheTokenPair> GetContentItemDependencies(dynamic item)
        {
            var dependencies = new List<CacheTokenPair>();
            KenticoCloudCacheHelper.ExtractCodenamesFromItem(item, out string extractedItemCodename, out string extractedTypeCodename);

            if (!string.IsNullOrEmpty(extractedItemCodename))
            {
                // Dependency on all formats of the item.
                dependencies.AddNonNullRange(
                    GetDependentIdentifierPairs(KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER, extractedItemCodename, identifierSet =>
                        Enumerable.Repeat(identifierSet, 1)
                    ));

                // Dependency on elements of item's type (if possible).
                dependencies.AddNonNullRange(GetContentTypeDependencies(KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, extractedTypeCodename));

                // Dependency on item's taxonomy elements.
                foreach (string taxonomyElementCodename in KenticoCloudCacheHelper.GetItemTaxonomyCodenamesByElements(item))
                {
                    dependencies.AddNonNullRange(
                        GetTaxonomyDependencies(KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, taxonomyElementCodename)
                    );
                }
            }

            return dependencies;
        }

        private IEnumerable<CacheTokenPair> GetTaxonomyDependencies(string originalFormatIdentifier, string taxonomyCodename)
        {
            return GetDependentIdentifierPairs(
                originalFormatIdentifier, taxonomyCodename, identifierSet =>
                    Enumerable.Repeat(identifierSet, 1)
            );
        }

        private IEnumerable<CacheTokenPair> GetContentTypeDependencies(string typeName, string codeName, dynamic response = null)
        {
            var dependencies = new List<CacheTokenPair>();

            dependencies.AddNonNullRange(
                GetDependentIdentifierPairs(typeName, codeName, identifierSet =>
                    Enumerable.Repeat(identifierSet, 1)
                ));

            // Try to get element codenames from the response.
            if (response != null)
            {
                if (response is ContentType && response?.Elements != null)
                {
                    foreach (var element in response.Elements)
                    {
                        dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentElementDependenciesInternal(KenticoCloudCacheHelper.CONTENT_ELEMENT_IDENTIFIER, element.Value));
                    }
                }
                else if (response is JObject)
                {
                    var elements = response?[KenticoCloudCacheHelper.ELEMENTS_IDENTIFIER];

                    if (elements != null)
                    {
                        foreach (var element in elements)
                        {
                            dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetContentElementDependenciesInternal(KenticoCloudCacheHelper.CONTENT_ELEMENT_JSON_IDENTIFIER, element));
                        }
                    }
                }
            }
            // If no response exists, try to get element codenames from the cache.
            else
            {
                dependencies.AddNonNullRange(
                    GetDependentIdentifierPairs(
                        typeName, codeName, cacheTokenPair =>
                        {
                            return _cacheManager.GetDependenciesFromCache<ContentType>(cacheTokenPair, cachedContentType =>
                            {
                                var dependenciesPerCacheEntry = new List<CacheTokenPair>();

                                foreach (var elementDependency in cachedContentType?.Elements?.SelectMany(e => GetContentElementDependencies(e.Value)))
                                {
                                    dependenciesPerCacheEntry.Add(elementDependency);
                                }

                                if (!string.IsNullOrEmpty(cacheTokenPair.TypeName))
                                {
                                    dependenciesPerCacheEntry.Add(new CacheTokenPair
                                    {
                                        TypeName = cacheTokenPair.TypeName,
                                        Codename = cachedContentType.System?.Codename
                                    });
                                }

                                return dependenciesPerCacheEntry;
                            });
                        }
                ));
            }

            return dependencies;
        }

        private IEnumerable<CacheTokenPair> GetContentElementDependenciesInternal(string originalFormatIdentifier, dynamic response)
        {
            var dependencies = new List<CacheTokenPair>();
            string elementCodename = null;
            string elementType = null;

            if (response is ContentElement)
            {
                elementCodename = response?.Codename?.ToString();
                elementType = response?.Type?.ToString();
            }
            else if (response is JProperty)
            {
                elementCodename = response?.Name?.ToString();
                elementType = response?.Value?[KenticoCloudCacheHelper.TYPE_IDENTIFIER]?.ToString();
            }

            if (!string.IsNullOrEmpty(elementType) && !string.IsNullOrEmpty(elementCodename))
            {
                dependencies.AddNonNullRange(
                    GetDependentIdentifierPairs(
                        originalFormatIdentifier, elementCodename, cacheTokenPair =>
                            new List<CacheTokenPair>
                            {
                                new CacheTokenPair
                                {
                                    TypeName = cacheTokenPair.TypeName,
                                    Codename = string.Join("|", elementType, elementCodename)
                                }
                            }
                ));
            }

            return dependencies;
        }

        #endregion Dependency resolvers
    }
}