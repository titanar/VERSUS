using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using VERSUS.Kentico.Services;
using VERSUS.Kentico.Webhooks.Models;

namespace VERSUS.Kentico.Webhooks.Services
{
    public class WebhookListener<TModel> : IWebhookListener<TModel> where TModel : IWebhookSubjectModel
    {
        public Subject<TModel> WebhookObservable { get; }

        public static List<string> InvalidatingOperations => new List<string>
        {
            "upsert",
            "publish",
            "restore_publish",
            "unpublish",
            "archive",
            "restore"
        };

        public WebhookListener(ICacheManager cacheManager)
        {
            WebhookObservable = new Subject<TModel>();

            WebhookObservable
                .Where(e => InvalidatingOperations.Any(operation => operation.Equals(e.Operation, StringComparison.Ordinal)))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(e => cacheManager.InvalidateEntry(e.TypeName, e.Codename));
        }
    }
}