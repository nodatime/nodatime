#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
