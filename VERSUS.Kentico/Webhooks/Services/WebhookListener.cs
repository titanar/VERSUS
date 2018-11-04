using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Services;
using VERSUS.Kentico.Webhooks.Models;

namespace VERSUS.Kentico.Webhooks.Services
{
    public class WebhookListener : IWebhookListener
    {
        public Subject<CacheInvalidationModel> WebhookObservable { get; }

        public WebhookListener(ICacheManager cacheManager)
        {
            WebhookObservable = new Subject<CacheInvalidationModel>();

            WebhookObservable
                .Where(e => KenticoCloudCacheHelper.InvalidatingOperations.Any(operation => operation.Equals(e.Operation, StringComparison.Ordinal)))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(e => cacheManager.InvalidateEntry(e.IdentifierSet));
        }
    }
}