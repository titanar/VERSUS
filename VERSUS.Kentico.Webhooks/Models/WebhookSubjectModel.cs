using System;

namespace VERSUS.Kentico.Webhooks.Models
{
    public class WebhookSubjectModel : IEquatable<WebhookSubjectModel>, IWebhookSubjectModel
    {
        public string TypeName { get; set; }

        public string Codename { get; set; }

        public string Operation { get; set; }

        public bool Equals(WebhookSubjectModel other)
        {
            if (other != null && ReferenceEquals(this, other))
            {
                return true;
            }

            return Operation.Equals(other.Operation, StringComparison.Ordinal) &&
                    TypeName.Equals(other.TypeName, StringComparison.Ordinal) &&
                    Codename.Equals(other.Codename, StringComparison.Ordinal);
        }
    }
}