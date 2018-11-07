namespace VERSUS.Kentico.Webhooks.Models
{
    public interface IWebhookSubjectModel
    {
        string TypeName { get; set; }

        string Codename { get; set; }

        string Operation { get; set; }
    }
}