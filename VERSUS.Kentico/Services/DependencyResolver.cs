using System;
using System.Collections.Generic;
using System.Linq;
using KenticoCloud.Delivery;
using Newtonsoft.Json.Linq;
using VERSUS.Core.Extensions;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Services.Models;

namespace VERSUS.Kentico.Services
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly ICacheManager _cacheManager;

        public DependencyResolver(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #region Public methods

        /// <summary>
        /// Extracts identifier sets from a single-item response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryItemResponse"/> or <see cref="DeliveryItemResponse{T}"/>, either strongly-typed, or runtime-typed.</param>
        /// <returns>Identifiers of all formats of the item, its modular content items, taxonomies used in elements, underlying content type and eventually its elements (when present in the cache).</returns>
        public IEnumerable<CacheTokenPair> GetItemResponseDependencies(dynamic response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemResponse(response) && response?.Item != null)
            {
                dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetItemDependencies(response.Item));

                foreach (var item in response.LinkedItems)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single-item JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> response.</param>
        /// <returns>Identifiers of all formats of the item, its modular content items, taxonomies used in elements, underlying content type and eventually its elements (when present in the cache).</returns>
        public IEnumerable<CacheTokenPair> GetItemJsonResponseDependencies(JObject response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemJsonResponse(response))
            {
                dependencies.AddNonNullRange(GetItemDependencies(response[KenticoCloudCacheHelper.ITEM_IDENTIFIER]));

                foreach (var item in response[KenticoCloudCacheHelper.MODULAR_CONTENT_IDENTIFIER]?.Children())
                {
                    dependencies.AddNonNullRange(GetItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from an item listing response.
        /// </summary>
        /// <param name="response">The <see cref="DeliveryItemListingResponse"/> or <see cref="DeliveryItemListingResponse{T}"/>, either strongly-typed, or runtime-typed.</param>
        /// <returns>Identifiers of all formats of the items, their modular content items, taxonomies used in elements, underlying content types and eventually their elements (when present in the cache).</returns>
        public IEnumerable<CacheTokenPair> GetItemListingResponseDependencies(dynamic response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemListingResponse(response))
            {
                foreach (dynamic item in response.Items)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetItemDependencies(item));
                }

                foreach (var item in response.LinkedItems)
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a item listing JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> item listing response.</param>
        /// <returns>Identifiers of all formats of the items, their modular content items, taxonomies used in elements, underlying content types and eventually their elements (when present in the cache).</returns>
        public IEnumerable<CacheTokenPair> GetItemListingJsonResponseDependencies(JObject response)
        {
            var dependencies = new List<CacheTokenPair>();

            if (KenticoCloudCacheHelper.IsDeliveryItemListingJsonResponse(response))
            {
                foreach (dynamic item in response[KenticoCloudCacheHelper.ITEMS_IDENTIFIER].Children())
                {
                    dependencies.AddNonNullRange((IEnumerable<CacheTokenPair>)GetItemDependencies(item));
                }

                foreach (var item in response[KenticoCloudCacheHelper.MODULAR_CONTENT_IDENTIFIER]?.Children())
                {
                    dependencies.AddNonNullRange(GetItemDependencies(item));
                }
            }

            return dependencies.Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content element response.
        /// </summary>
        /// <param name="response">The <see cref="ContentElement"/> response.</param>
        /// <returns>Identifiers of all formats of the element.</returns>
        public IEnumerable<CacheTokenPair> GetContentElementDependencies(ContentElement response)
        {
            return GetContentElementDependenciesInternal(KenticoCloudCacheHelper.CONTENT_ELEMENT_IDENTIFIER, response).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single taxonomy group response.
        /// </summary>
        /// <param name="response">The <see cref="TaxonomyGroup"/> response.</param>
        /// <returns>Identifiers of all formats of the taxonomy.</returns>
        public IEnumerable<CacheTokenPair> GetTaxonomySingleDependency(TaxonomyGroup response)
        {
            return GetTaxonomyDependencies(KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, response?.System?.Codename).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a single taxonomy group JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> taxonomy response.</param>
        /// <returns>Identifiers of all formats of the taxonomy.</returns>
        public IEnumerable<CacheTokenPair> GetTaxonomySingleJsonDependency(JObject response)
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
        public IEnumerable<CacheTokenPair> GetTaxonomyListingDependencies(DeliveryTaxonomyListingResponse response)
        {
            return response?.Taxonomies?.SelectMany(t => GetTaxonomySingleDependency(t)).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a listing taxonomy group JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> response.</param>
        /// <returns>Identifiers of all formats of all the taxonomies.</returns>
        public IEnumerable<CacheTokenPair> GetTaxonomyListingJsonDependencies(JObject response)
        {
            return response?[KenticoCloudCacheHelper.TAXONOMIES_IDENTIFIER]?.SelectMany(t => GetTaxonomySingleJsonDependency(t.ToObject<JObject>())).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type response.
        /// </summary>
        /// <param name="response">The <see cref="ContentType"/> response.</param>
        /// <returns>Identifiers of all formats of the content type and its elements.</returns>
        public IEnumerable<CacheTokenPair> GetTypeSingleDependencies(ContentType response)
        {
            return GetContentTypeDependencies(KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, response?.System?.Codename, response).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> content type response.</param>
        /// <returns>Identifiers of all formats of the content type and its elements.</returns>
        public IEnumerable<CacheTokenPair> GetTypeSingleJsonDependencies(JObject response)
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
        public IEnumerable<CacheTokenPair> GetTypeListingDependencies(DeliveryTypeListingResponse response)
        {
            return response?.Types?.SelectMany(t => GetTypeSingleDependencies(t)).Distinct();
        }

        /// <summary>
        /// Extracts identifier sets from a content type listing JSON response.
        /// </summary>
        /// <param name="response">The <see cref="JObject"/> content type listing response.</param>
        /// <returns>Identifiers of all formats of all the content types and their elements.</returns>
        public IEnumerable<CacheTokenPair> GetTypeListingJsonDependencies(JObject response)
        {
            return response?[KenticoCloudCacheHelper.TYPES_IDENTIFIER]?.SelectMany(t => GetTypeSingleJsonDependencies(t.ToObject<JObject>())).Distinct();
        }

        #endregion Public methods

        #region Private methods

        private IEnumerable<CacheTokenPair> GetDependentTokenPairs(string typeName, string codename, Func<CacheTokenPair, IEnumerable<CacheTokenPair>> dependencyFactory)
        {
            foreach (var dependentTypeName in KenticoCloudCacheHelper.GetDependentTypeNames(typeName))
            {
                return dependencyFactory(new CacheTokenPair(dependentTypeName, codename));
            }

            return null;
        }

        private IEnumerable<CacheTokenPair> GetItemDependencies(dynamic item)
        {
            var dependencies = new List<CacheTokenPair>();
            ExtractCodenamesFromItem(item, out string extractedItemCodename, out string extractedTypeCodename);

            if (!string.IsNullOrEmpty(extractedItemCodename))
            {
                // Dependency on all formats of the item.
                dependencies.AddNonNullRange(
                    GetDependentTokenPairs(KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER, extractedItemCodename, cacheTokenPair => new[] { cacheTokenPair }
                ));

                // Dependency on elements of item's type (if possible).
                dependencies.AddNonNullRange(GetContentTypeDependencies(KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER, extractedTypeCodename));

                // Dependency on item's taxonomy elements.
                foreach (string taxonomyElementCodename in GetItemTaxonomyCodenamesByElements(item))
                {
                    dependencies.AddNonNullRange(
                        GetTaxonomyDependencies(KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, taxonomyElementCodename)
                    );
                }
            }

            return dependencies;
        }

        private IEnumerable<CacheTokenPair> GetTaxonomyDependencies(string typeName, string taxonomyCodename)
        {
            return GetDependentTokenPairs(
                typeName, taxonomyCodename, cacheTokenPair => new[] { cacheTokenPair }
            );
        }

        private IEnumerable<CacheTokenPair> GetContentTypeDependencies(string typeName, string codeName, dynamic response = null)
        {
            var dependencies = new List<CacheTokenPair>();

            dependencies.AddNonNullRange(
                GetDependentTokenPairs(typeName, codeName, cacheTokenPair => new[] { cacheTokenPair }
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
                    GetDependentTokenPairs(
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
                                    dependenciesPerCacheEntry.Add(new CacheTokenPair(cacheTokenPair.TypeName, cachedContentType.System?.Codename));
                                }

                                return dependenciesPerCacheEntry;
                            });
                        }
                ));
            }

            return dependencies;
        }

        private IEnumerable<CacheTokenPair> GetContentElementDependenciesInternal(string typeName, dynamic response)
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
                    GetDependentTokenPairs(
                        typeName, elementCodename, cacheTokenPair =>
                            new[]
                            {
                                new CacheTokenPair(cacheTokenPair.TypeName, string.Join("|", elementType, elementCodename))
                            }
                ));
            }

            return dependencies;
        }

        private static IEnumerable<string> GetItemJsonTaxonomyCodenamesByElements(JToken elementsToken)
        {
            var taxonomyElements = elementsToken?
                                    .SelectMany(t => t.Children())?
                                    .Where(
                                        e => e[KenticoCloudCacheHelper.TYPE_IDENTIFIER] != null &&
                                        e[KenticoCloudCacheHelper.TYPE_IDENTIFIER].ToString().Equals(KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER, StringComparison.Ordinal) &&
                                        e[KenticoCloudCacheHelper.TAXONOMY_GROUP_IDENTIFIER] != null &&
                                        !string.IsNullOrEmpty(e[KenticoCloudCacheHelper.TAXONOMY_GROUP_IDENTIFIER].ToString())
                                    );

            return taxonomyElements.Select(e => e[KenticoCloudCacheHelper.TAXONOMY_GROUP_IDENTIFIER].ToString());
        }

        private static IEnumerable<string> GetItemTaxonomyCodenamesByElements(dynamic item)
        {
            if (item is ContentItem)
            {
                return GetItemJsonTaxonomyCodenamesByElements(item?.Elements);
            }
            else if (item is JObject)
            {
                return GetItemJsonTaxonomyCodenamesByElements(item?[KenticoCloudCacheHelper.ELEMENTS_IDENTIFIER]);
            }
            else
            {
                var codenames = new List<string>();
                var properties = item?.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (property.PropertyType.GenericTypeArguments.Length > 0 &&
                        property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                        property.PropertyType.GenericTypeArguments[0] == typeof(TaxonomyTerm))
                    {
                        var codenameProperty = item.GetType().GetField($"{property.Name}Codename");
                        codenames.Add(codenameProperty.GetValue(item) as string);
                    }
                }
                return codenames;
            }
        }

        private static void ExtractCodenamesFromItem(dynamic item, out string extractedItemCodename, out string extractedTypeCodename)
        {
            extractedItemCodename = null;
            extractedTypeCodename = null;

            if ((item is ContentItem || !(item is JProperty) && !(item is JObject)) && item?.System != null)
            {
                extractedItemCodename = item.System.Codename?.ToString();
                extractedTypeCodename = item.System.Type?.ToString();
            }
            else if (item is JProperty && item?.Value?[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER] != null)
            {
                extractedItemCodename = item.Value[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.CODENAME_IDENTIFIER]?.ToString();
                extractedTypeCodename = item.Value[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.TYPE_IDENTIFIER]?.ToString();
            }
            else if (item is JObject && item[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER] != null)
            {
                extractedItemCodename = item?[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.CODENAME_IDENTIFIER]?.ToString();
                extractedTypeCodename = item?[KenticoCloudCacheHelper.SYSTEM_IDENTIFIER][KenticoCloudCacheHelper.TYPE_IDENTIFIER]?.ToString();
            }
        }

        #endregion Private methods
    }
}