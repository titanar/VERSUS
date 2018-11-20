using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Newtonsoft.Json.Linq;

using VERSUS.Core.Extensions;
using VERSUS.Kentico.Helpers;

namespace VERSUS.Kentico.Services
{
    public class CachedDeliveryClient : IDeliveryClient
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDeliveryClient _deliveryClient;
        private readonly IDeliveryClient _previewDeliveryClient;
        private readonly IDependencyResolver _dependencyResolver;

        #region Constructors

        public CachedDeliveryClient(ICacheManager cacheManager, IDeliveryClient deliveryClient, IDeliveryClient previewDeliveryClient, IDependencyResolver dependencyResolver)
        {
            _cacheManager = cacheManager;
            _deliveryClient = deliveryClient;
            _previewDeliveryClient = previewDeliveryClient;
            _dependencyResolver = dependencyResolver;
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_JSON_IDENTIFIER, codename };
            cacheTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemJsonAsync(codename, parameters),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetItemJsonAsync(codename, parameters),
                response => response == null,
                _dependencyResolver.GetItemJsonResponseDependencies);
        }

        /// <summary>
        /// Returns content items as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for filtering, ordering or depth of modular content.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content items. If no query parameters are specified, all content items are returned.</returns>
        public async Task<JObject> GetItemsJsonAsync(params string[] parameters)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_JSON_IDENTIFIER };
            cacheTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemsJsonAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetItemsJsonAsync(parameters),
                response => response["items"].Count() <= 0,
                _dependencyResolver.GetItemListingJsonResponseDependencies);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER, codename };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemAsync(codename, parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryItemResponse>>)null : () => _previewDeliveryClient.GetItemAsync(codename, parameters),
                response => response == null,
                _dependencyResolver.GetItemResponseDependencies);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_TYPED_IDENTIFIER, codename };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemAsync<T>(codename, parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryItemResponse<T>>>)null : () => _previewDeliveryClient.GetItemAsync<T>(codename, parameters),
                response => response == null,
                _dependencyResolver.GetItemResponseDependencies);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_IDENTIFIER };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemsAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryItemListingResponse>>)null : () => _previewDeliveryClient.GetItemsAsync(parameters),
                response => response.Items.Count <= 0,
                _dependencyResolver.GetItemListingResponseDependencies);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ITEM_LISTING_TYPED_IDENTIFIER };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetItemsAsync<T>(parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryItemListingResponse<T>>>)null : () => _previewDeliveryClient.GetItemsAsync<T>(parameters),
                response => response.Items.Count <= 0,
                _dependencyResolver.GetItemListingResponseDependencies);
        }

        /// <summary>
        /// Returns a content type as JSON data.
        /// </summary>
        /// <param name="codename">The codename of a content type.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content type with the specified codename.</returns>
        public async Task<JObject> GetTypeJsonAsync(string codename)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_JSON_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTypeJsonAsync(codename),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetTypeJsonAsync(codename),
                response => response == null,
                _dependencyResolver.GetTypeSingleJsonDependencies);
        }

        /// <summary>
        /// Returns content types as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example for paging.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the content types. If no query parameters are specified, all content types are returned.</returns>
        public async Task<JObject> GetTypesJsonAsync(params string[] parameters)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_LISTING_JSON_IDENTIFIER };
            cacheTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTypesJsonAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetTypesJsonAsync(parameters),
                response => response["types"].Count() <= 0,
                _dependencyResolver.GetTypeListingJsonDependencies);
        }

        /// <summary>
        /// Returns a content type.
        /// </summary>
        /// <param name="codename">The codename of a content type.</param>
        /// <returns>The content type with the specified codename.</returns>
        public async Task<ContentType> GetTypeAsync(string codename)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTypeAsync(codename),
                _previewDeliveryClient == null ? (Func<Task<ContentType>>)null : () => _previewDeliveryClient.GetTypeAsync(codename),
                response => response == null,
                _dependencyResolver.GetTypeSingleDependencies);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_TYPE_LISTING_IDENTIFIER };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTypesAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryTypeListingResponse>>)null : () => _previewDeliveryClient.GetTypesAsync(parameters),
                response => response.Types.Count <= 0,
                _dependencyResolver.GetTypeListingDependencies);
        }

        /// <summary>
        /// Returns a content element.
        /// </summary>
        /// <param name="contentTypeCodename">The codename of the content type.</param>
        /// <param name="contentElementCodename">The codename of the content element.</param>
        /// <returns>A content element with the specified codename that is a part of a content type with the specified codename.</returns>
        public async Task<ContentElement> GetContentElementAsync(string contentTypeCodename, string contentElementCodename)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.CONTENT_ELEMENT_IDENTIFIER, contentTypeCodename, contentElementCodename };

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetContentElementAsync(contentTypeCodename, contentElementCodename),
                _previewDeliveryClient == null ? (Func<Task<ContentElement>>)null : () => _previewDeliveryClient.GetContentElementAsync(contentTypeCodename, contentElementCodename),
                response => response == null,
                _dependencyResolver.GetContentElementDependencies);
        }

        /// <summary>
        /// Returns a taxonomy group as JSON data.
        /// </summary>
        /// <param name="codename">The codename of a taxonomy group.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the taxonomy group with the specified codename.</returns>
        public async Task<JObject> GetTaxonomyJsonAsync(string codename)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTaxonomyJsonAsync(codename),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetTaxonomyJsonAsync(codename),
                response => response == null,
                _dependencyResolver.GetTaxonomySingleJsonDependency);
        }

        /// <summary>
        /// Returns taxonomy groups as JSON data.
        /// </summary>
        /// <param name="parameters">An array that contains zero or more query parameters, for example, for paging.</param>
        /// <returns>The <see cref="JObject"/> instance that represents the taxonomy groups. If no query parameters are specified, all taxonomy groups are returned.</returns>
        public async Task<JObject> GetTaxonomiesJsonAsync(params string[] parameters)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_LISTING_JSON_IDENTIFIER };
            cacheTokens.AddNonNullRange(parameters);

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTaxonomiesJsonAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<JObject>>)null : () => _previewDeliveryClient.GetTaxonomiesJsonAsync(parameters),
                response => response["taxonomies"].Count() <= 0,
                _dependencyResolver.GetTaxonomyListingJsonDependencies);
        }

        /// <summary>
        /// Returns a taxonomy group.
        /// </summary>
        /// <param name="codename">The codename of a taxonomy group.</param>
        /// <returns>The taxonomy group with the specified codename.</returns>
        public async Task<TaxonomyGroup> GetTaxonomyAsync(string codename)
        {
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, codename };

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTaxonomyAsync(codename),
                _previewDeliveryClient == null ? (Func<Task<TaxonomyGroup>>)null : () => _previewDeliveryClient.GetTaxonomyAsync(codename),
                response => response == null,
                _dependencyResolver.GetTaxonomySingleDependency);
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
            var cacheTokens = new List<string> { KenticoCloudCacheHelper.TAXONOMY_GROUP_LISTING_IDENTIFIER };
            cacheTokens.AddNonNullRange(KenticoCloudCacheHelper.GetIdentifiersFromParameters(parameters));

            return await _cacheManager.GetOrCreateAsync(
                cacheTokens,
                () => _deliveryClient.GetTaxonomiesAsync(parameters),
                _previewDeliveryClient == null ? (Func<Task<DeliveryTaxonomyListingResponse>>)null : () => _previewDeliveryClient.GetTaxonomiesAsync(parameters),
                response => response.Taxonomies.Count <= 0,
                _dependencyResolver.GetTaxonomyListingDependencies);
        }

        #endregion Public methods
    }
}