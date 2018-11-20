using System.Collections.Generic;
using System.Linq;

using KenticoCloud.Delivery;

using Newtonsoft.Json.Linq;

namespace VERSUS.Kentico.Helpers
{
    public static class KenticoCloudCacheHelper
    {
        #region Constants

        public const string CONTENT_ITEM_SINGLE_IDENTIFIER = "content_item";
        public const string CONTENT_ITEM_SINGLE_JSON_IDENTIFIER = CONTENT_ITEM_SINGLE_IDENTIFIER + JSON_SUFFIX;
        public const string CONTENT_ITEM_SINGLE_TYPED_IDENTIFIER = CONTENT_ITEM_SINGLE_IDENTIFIER + TYPED_SUFFIX;
        public const string CONTENT_ITEM_SINGLE_RUNTIME_TYPED_IDENTIFIER = CONTENT_ITEM_SINGLE_IDENTIFIER + RUNTIME_TYPED_SUFFIX;
        public const string CONTENT_ITEM_VARIANT_SINGLE_IDENTIFIER = CONTENT_ITEM_SINGLE_IDENTIFIER + "_variant";
        public const string CONTENT_ITEM_LISTING_IDENTIFIER = CONTENT_ITEM_SINGLE_IDENTIFIER + LISTING_SUFFIX;
        public const string CONTENT_ITEM_LISTING_JSON_IDENTIFIER = CONTENT_ITEM_LISTING_IDENTIFIER + JSON_SUFFIX;
        public const string CONTENT_ITEM_LISTING_TYPED_IDENTIFIER = CONTENT_ITEM_LISTING_IDENTIFIER + TYPED_SUFFIX;
        public const string CONTENT_ITEM_LISTING_RUNTIME_TYPED_IDENTIFIER = CONTENT_ITEM_LISTING_IDENTIFIER + RUNTIME_TYPED_SUFFIX;
        public const string CONTENT_TYPE_SINGLE_IDENTIFIER = "content_type";
        public const string CONTENT_TYPE_SINGLE_JSON_IDENTIFIER = CONTENT_TYPE_SINGLE_IDENTIFIER + JSON_SUFFIX;
        public const string CONTENT_TYPE_LISTING_IDENTIFIER = CONTENT_TYPE_SINGLE_IDENTIFIER + LISTING_SUFFIX;
        public const string CONTENT_TYPE_LISTING_JSON_IDENTIFIER = CONTENT_TYPE_LISTING_IDENTIFIER + JSON_SUFFIX;
        public const string CONTENT_ELEMENT_IDENTIFIER = "content_element";
        public const string CONTENT_ELEMENT_JSON_IDENTIFIER = CONTENT_ELEMENT_IDENTIFIER + JSON_SUFFIX;
        public const string TAXONOMY_GROUP_SINGLE_IDENTIFIER = "taxonomy";
        public const string TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER = TAXONOMY_GROUP_SINGLE_IDENTIFIER + JSON_SUFFIX;
        public const string TAXONOMY_GROUP_LISTING_IDENTIFIER = TAXONOMY_GROUP_SINGLE_IDENTIFIER + LISTING_SUFFIX;
        public const string TAXONOMY_GROUP_LISTING_JSON_IDENTIFIER = TAXONOMY_GROUP_LISTING_IDENTIFIER + JSON_SUFFIX;

        public const string CODENAME_IDENTIFIER = "codename";
        public const string SYSTEM_IDENTIFIER = "system";
        public const string MODULAR_CONTENT_IDENTIFIER = "modular_content";
        public const string ITEM_IDENTIFIER = "item";
        public const string ITEMS_IDENTIFIER = "items";
        public const string TYPE_IDENTIFIER = "type";
        public const string TYPES_IDENTIFIER = "types";
        public const string TAXONOMIES_IDENTIFIER = "taxonomies";
        public const string ELEMENTS_IDENTIFIER = "elements";
        public const string TAXONOMY_GROUP_IDENTIFIER = "taxonomy_group";

        private const string LISTING_SUFFIX = "_listing";
        private const string JSON_SUFFIX = "_json";
        private const string TYPED_SUFFIX = "_typed";
        private const string RUNTIME_TYPED_SUFFIX = "_runtime_typed";

        private static readonly Dictionary<string, IEnumerable<string>> RelatedFormats = new Dictionary<string, IEnumerable<string>>()
            {
                { CONTENT_ITEM_SINGLE_IDENTIFIER, ContentItemSingleRelatedFormats },
                { CONTENT_ITEM_SINGLE_JSON_IDENTIFIER, ContentItemSingleRelatedFormats },
                { CONTENT_ITEM_SINGLE_TYPED_IDENTIFIER, ContentItemSingleRelatedFormats },
                { CONTENT_ITEM_SINGLE_RUNTIME_TYPED_IDENTIFIER, ContentItemSingleRelatedFormats },
                { CONTENT_ITEM_VARIANT_SINGLE_IDENTIFIER, ContentItemSingleRelatedFormats },
                { CONTENT_ITEM_LISTING_IDENTIFIER, ContentItemListingRelatedFormats },
                { CONTENT_ITEM_LISTING_JSON_IDENTIFIER, ContentItemListingRelatedFormats },
                { CONTENT_ITEM_LISTING_TYPED_IDENTIFIER, ContentItemListingRelatedFormats },
                { CONTENT_ITEM_LISTING_RUNTIME_TYPED_IDENTIFIER, ContentItemListingRelatedFormats },
                { CONTENT_TYPE_SINGLE_IDENTIFIER, ContentTypeSingleRelatedFormats },
                { CONTENT_TYPE_SINGLE_JSON_IDENTIFIER, ContentTypeSingleRelatedFormats },
                { CONTENT_TYPE_LISTING_IDENTIFIER, ContentTypeListingRelatedFormats },
                { CONTENT_TYPE_LISTING_JSON_IDENTIFIER, ContentTypeListingRelatedFormats },
                { CONTENT_ELEMENT_IDENTIFIER, ContentElementRelatedFormats },
                { CONTENT_ELEMENT_JSON_IDENTIFIER, ContentElementRelatedFormats },
                { TAXONOMY_GROUP_SINGLE_IDENTIFIER, TaxonomyGroupSingleRelatedFormats },
                { TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER, TaxonomyGroupSingleRelatedFormats },
                { TAXONOMY_GROUP_LISTING_IDENTIFIER, TaxonomyGroupListingRelatedFormats },
                { TAXONOMY_GROUP_LISTING_JSON_IDENTIFIER, TaxonomyGroupListingRelatedFormats }
            };

        #endregion Constants

        #region Properties

        public static IEnumerable<string> ContentItemSingleRelatedFormats => new[]
        {
            CONTENT_ITEM_SINGLE_IDENTIFIER,
            CONTENT_ITEM_SINGLE_JSON_IDENTIFIER,
            CONTENT_ITEM_SINGLE_TYPED_IDENTIFIER,
            CONTENT_ITEM_SINGLE_RUNTIME_TYPED_IDENTIFIER,
            CONTENT_ITEM_VARIANT_SINGLE_IDENTIFIER,
        };

        public static IEnumerable<string> ContentItemListingRelatedFormats => new[]
        {
            CONTENT_ITEM_LISTING_IDENTIFIER,
            CONTENT_ITEM_LISTING_JSON_IDENTIFIER,
            CONTENT_ITEM_LISTING_TYPED_IDENTIFIER,
            CONTENT_ITEM_LISTING_RUNTIME_TYPED_IDENTIFIER
        };

        public static IEnumerable<string> ContentTypeSingleRelatedFormats => new[]
        {
            CONTENT_TYPE_SINGLE_IDENTIFIER,
            CONTENT_TYPE_SINGLE_JSON_IDENTIFIER
        };

        public static IEnumerable<string> ContentTypeListingRelatedFormats => new[]
        {
            CONTENT_TYPE_LISTING_IDENTIFIER,
            CONTENT_TYPE_LISTING_JSON_IDENTIFIER
        };

        public static IEnumerable<string> ContentElementRelatedFormats => new[]
        {
            CONTENT_ELEMENT_IDENTIFIER,
            CONTENT_ELEMENT_JSON_IDENTIFIER
        };

        public static IEnumerable<string> TaxonomyGroupSingleRelatedFormats => new[]
        {
            TAXONOMY_GROUP_SINGLE_IDENTIFIER,
            TAXONOMY_GROUP_SINGLE_JSON_IDENTIFIER
        };

        public static IEnumerable<string> TaxonomyGroupListingRelatedFormats => new[]
        {
            TAXONOMY_GROUP_LISTING_IDENTIFIER,
            TAXONOMY_GROUP_LISTING_JSON_IDENTIFIER
        };

        #endregion Properties

        #region Public methods

        public static IEnumerable<string> GetIdentifiersFromParameters(IEnumerable<IQueryParameter> parameters)
        {
            return parameters?.Select(p => p.GetQueryStringParameter());
        }

        public static bool IsDeliveryItemResponse(dynamic response)
        {
            return (response is DeliveryItemResponse ||
                (response.GetType().IsGenericType &&
                response.GetType().GetGenericTypeDefinition() == typeof(DeliveryItemResponse<>))) ? true : false;
        }

        public static bool IsDeliveryItemListingResponse(dynamic response)
        {
            return (response is DeliveryItemListingResponse ||
                (response.GetType().IsGenericType &&
                response.GetType().GetGenericTypeDefinition() == typeof(DeliveryItemListingResponse<>))) ? true : false;
        }

        public static bool IsDeliveryItemJsonResponse(JObject response)
        {
            return (response?[ITEM_IDENTIFIER] != null) ? true : false;
        }

        public static bool IsDeliveryItemListingJsonResponse(JObject response)
        {
            return (response?[ITEMS_IDENTIFIER] != null) ? true : false;
        }

        public static IEnumerable<string> GetDependentTypeNames(string typeCodeName)
        {
            return RelatedFormats[typeCodeName];
        }

        #endregion Public methods
    }
}