using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NodaTime.Utility
{
    /// <summary>
    /// An equality comparer which compares references for equality and uses the "original" object hash code
    /// for hash codes.
    /// </summary>
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T first, T second)
        {
            return ReferenceEquals(first, second);
        }

        public int GetHashCode(T value)
        {
            return ReferenceEquals(value, null) ? 0 : RuntimeHelpers.GetHashCode(value);
        }
    }
}
