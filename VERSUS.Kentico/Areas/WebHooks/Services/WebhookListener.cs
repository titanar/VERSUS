using System;
using System.Linq;
using System.Reactive.Linq;

using VERSUS.Kentico.Areas.WebHooks.Models;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Services;

namespace VERSUS.Kentico.Areas.WebHooks.Services
{
    public class WebhookListener : IWebhookListener
    {
        public event EventHandler<CacheInvalidationEventArgs> WebhookNotification = delegate { };

        public WebhookListener(ICacheManager cacheManager)
        {
            Observable.FromEventPattern<CacheInvalidationEventArgs>(this, nameof(WebhookNotification))
                .Where(e => KenticoCloudCacheHelper.InvalidatingOperations.Any(operation => operation.Equals(e.EventArgs.Operation, StringComparison.Ordinal)))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(e => cacheManager.InvalidateEntry(e.EventArgs.IdentifierSet));
        }

        public void RaiseWebhookNotification(object sender, string operation, CacheTokenPair identifierSet)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (string.IsNullOrEmpty(operation))
            {
                throw new ArgumentException("The 'operation' parameter must be a non-empty string.", nameof(operation));
            }

            if (identifierSet == null)
            {
                throw new ArgumentNullException(nameof(identifierSet));
            }

            WebhookNotification(sender, new CacheInvalidationEventArgs(identifierSet, operation));
        }
    }
}