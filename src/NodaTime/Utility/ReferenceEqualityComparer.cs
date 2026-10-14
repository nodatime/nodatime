// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NodaTime.Utility
{
    /// <summary>
    /// An equality comparer which compares references for equality and uses the "original" object hash code
    /// for hash codes. This is broadly equivalent to System.Collections.Generic.ReferenceEqualityComparer, but that's
    /// only available from .NET 5.
    /// </summary>
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T? first, T? second) => ReferenceEquals(first, second);

        public int GetHashCode(T value) => value is null ? 0 : RuntimeHelpers.GetHashCode(value);
    }
}
