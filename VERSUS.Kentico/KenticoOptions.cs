using System.Linq;
using KenticoCloud.Delivery;

namespace VERSUS.Core
{
    public class KenticoOptions : DeliveryOptions
    {
        public int CacheTimeoutSeconds { get; set; }

        public string KenticoCloudWebhookSecret { get; set; }

        public string KenticoCloudWebhookEndpoint { get; set; }

        public bool CreateCacheEntriesInBackground { get; set; }

        public int[] ResponsiveWidths { get; set; }

        public bool ResponsiveImagesEnabled => ResponsiveWidths != null && ResponsiveWidths.Count() > 0;

        public string KenticoCloudUrlSlugEndpoint { get; set; }
    }
}