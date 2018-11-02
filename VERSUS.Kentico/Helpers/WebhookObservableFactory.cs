using System;
using System.Linq;
using System.Reactive.Linq;
using VERSUS.Kentico.Areas.WebHooks.Models;

namespace VERSUS.Kentico.Helpers
{
    public static class WebhookObservableFactory
    {
        public static IObservable<CacheInvalidationEventArgs> GetObservable(object sender, string eventName)
        {
            return Observable.FromEventPattern<CacheInvalidationEventArgs>(sender, eventName).Select(e => e.EventArgs);
        }
    }
}
