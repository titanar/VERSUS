using System;

namespace VERSUS.Kentico.Services.Models
{
    public class CacheTokenPair : IEquatable<CacheTokenPair>
    {
        public string TypeName { get; set; }

        public string Codename { get; set; }

        public CacheTokenPair(string typeName, string codename)
        {
            TypeName = typeName;
            Codename = codename;
        }

        public bool Equals(CacheTokenPair other)
        {
            if (other != null && ReferenceEquals(this, other))
            {
                return true;
            }

            return TypeName.Equals(other.TypeName, StringComparison.Ordinal) && Codename.Equals(other.Codename, StringComparison.Ordinal);
        }
    }
}