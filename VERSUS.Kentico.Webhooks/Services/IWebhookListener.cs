using System.Reactive.Subjects;

namespace VERSUS.Kentico.Webhooks.Services
{
    public interface IWebhookListener<TModel>
    {
        Subject<TModel> WebhookObservable { get; }
    }
}