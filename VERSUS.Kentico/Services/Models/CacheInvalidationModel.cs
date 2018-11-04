using System;

namespace VERSUS.Kentico.Webhooks.Models
{
    public class CacheInvalidationModel : IEquatable<CacheInvalidationModel>
    {
        public CacheTokenPair IdentifierSet { get; }

        public string Operation { get; }

        public CacheInvalidationModel(CacheTokenPair identifierSet, string operation)
        {
            if (identifierSet != null && !string.IsNullOrEmpty(operation))
            {
                IdentifierSet = identifierSet;
                Operation = operation;
            }
        }

        public bool Equals(CacheInvalidationModel other)
        {
            if (other != null && ReferenceEquals(this, other))
            {
                return true;
            }

            return Operation.Equals(other.Operation, StringComparison.Ordinal) && IdentifierSet.Equals(other.IdentifierSet);
        }
    }
}