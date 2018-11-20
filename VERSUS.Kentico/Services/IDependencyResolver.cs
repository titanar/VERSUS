using System.Collections.Generic;
using KenticoCloud.Delivery;
using Newtonsoft.Json.Linq;
using VERSUS.Kentico.Services.Models;

namespace VERSUS.Kentico.Services
{
    public interface IDependencyResolver
    {
        IEnumerable<CacheTokenPair> GetItemResponseDependencies(dynamic response);

        IEnumerable<CacheTokenPair> GetItemJsonResponseDependencies(JObject response);

        IEnumerable<CacheTokenPair> GetItemListingResponseDependencies(dynamic response);

        IEnumerable<CacheTokenPair> GetItemListingJsonResponseDependencies(JObject response);

        IEnumerable<CacheTokenPair> GetContentElementDependencies(ContentElement response);

        IEnumerable<CacheTokenPair> GetTaxonomySingleDependency(TaxonomyGroup response);

        IEnumerable<CacheTokenPair> GetTaxonomySingleJsonDependency(JObject response);

        IEnumerable<CacheTokenPair> GetTaxonomyListingDependencies(DeliveryTaxonomyListingResponse response);

        IEnumerable<CacheTokenPair> GetTaxonomyListingJsonDependencies(JObject response);

        IEnumerable<CacheTokenPair> GetTypeSingleDependencies(ContentType response);

        IEnumerable<CacheTokenPair> GetTypeSingleJsonDependencies(JObject response);

        IEnumerable<CacheTokenPair> GetTypeListingDependencies(DeliveryTypeListingResponse response);

        IEnumerable<CacheTokenPair> GetTypeListingJsonDependencies(JObject response);
    }
}