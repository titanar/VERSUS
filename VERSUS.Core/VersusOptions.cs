using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VERSUS.Core
{
    public class VersusOptions
    {
        public string ConnectionString { get; set; }

        public int CommandTimeout { get; set; }

        public string ErrorHandlingRoute { get; set; }

        public int CacheTimeoutSeconds { get; set; }

        public string KenticoCloudWebhookSecret { get; set; }

        public bool CreateCacheEntriesInBackground { get; set; }

        public int[] ResponsiveWidths { get; set; }

        public bool ResponsiveImagesEnabled
        {
            get
            {
                return ResponsiveWidths != null && ResponsiveWidths.Count() > 0;
            }
        }
    }
}
