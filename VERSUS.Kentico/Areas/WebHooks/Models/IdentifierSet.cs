using System;

namespace VERSUS.Kentico.Areas.WebHooks.Models
{
    public class CacheIdentifierPair : IEquatable<CacheIdentifierPair>
    {
        public string TypeName { get; set; }
        public string Codename { get; set; }

        public bool Equals(CacheIdentifierPair other)
        {
            if (other != null && ReferenceEquals(this, other))
            {
                return true;
            }

            return TypeName.Equals(other.TypeName, StringComparison.Ordinal) && Codename.Equals(other.Codename, StringComparison.Ordinal);
        }
    }
}
