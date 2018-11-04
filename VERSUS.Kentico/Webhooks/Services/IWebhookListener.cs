using System.Reactive.Subjects;
using VERSUS.Kentico.Webhooks.Models;

namespace VERSUS.Kentico.Webhooks.Services
{
    /// <summary>
    /// Invokes an event, based on a webhook notification.
    /// </summary>
    public interface IWebhookListener
    {
        Subject<CacheInvalidationModel> WebhookObservable { get; }
    }
}