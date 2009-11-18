#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime
{
    /// <summary>
    /// A length of time in ticks. (There are 10,000 ticks in a millisecond.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is no concept of fields, such as days or seconds, as these fields
    /// can vary in length. A duration may be converted to an <see cref="IPeriod" /> to obtain
    /// field values. This conversion will typically cause a loss of precision.
    /// </para>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    public struct Duration
    {
        private readonly long ticks;

        /// <summary>
        /// The number of ticks in the duration.
        /// </summary>
        public long Ticks { get { return ticks; } }

        public Duration(long ticks)
        {
            this.ticks = ticks;
        }
    }
}
