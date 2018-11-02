using System;

namespace VERSUS.Kentico.Areas.WebHooks.Models
{
    public class CacheInvalidationEventArgs : EventArgs, IEquatable<CacheInvalidationEventArgs>
    {
        public CacheTokenPair IdentifierSet { get; }
        public string Operation { get; }

        public CacheInvalidationEventArgs(CacheTokenPair identifierSet, string operation)
        {
            if (identifierSet != null && !string.IsNullOrEmpty(operation))
            {
                IdentifierSet = identifierSet;
                Operation = operation;
            }
        }

        public bool Equals(CacheInvalidationEventArgs other)
        {
            if (other != null && ReferenceEquals(this, other))
            {
                return true;
            }

            return Operation.Equals(other.Operation, StringComparison.Ordinal) && IdentifierSet.Equals(other.IdentifierSet);
        }
    }
}