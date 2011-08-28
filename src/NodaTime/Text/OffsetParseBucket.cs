#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using System.Diagnostics;

namespace NodaTime.Text
{
    /// <summary>
    ///   Provides a container for the interim parsed pieces of an <see cref="Offset" /> value.
    /// </summary>
    [DebuggerStepThrough]
    internal class OffsetParseBucket : ParseBucket<Offset>
    {
        /// <summary>
        ///   The fractions of a seconds in milliseconds.
        /// </summary>
        internal int? FractionalSeconds;

        /// <summary>
        ///   The hours in the range [0, 23].
        /// </summary>
        internal int? Hours;

        /// <summary>
        ///   The total millisconds. This is the only value that can be negative.
        /// </summary>
        internal int? Milliseconds;

        /// <summary>
        ///   The minutes in the range [0, 59].
        /// </summary>
        internal int? Minutes;

        /// <summary>
        ///   The seconds in the range [0, 59].
        /// </summary>
        internal int? Seconds;

        /// <summary>
        ///   Gets a value indicating whether this instance is negative.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is negative; otherwise, <c>false</c>.
        /// </value>
        public bool IsNegative;

        /// <summary>
        ///   Calculates the value from the parsed pieces.
        /// </summary>
        internal override ParseResult<Offset> CalculateValue()
        {
            int hours = Hours ?? 0;
            int minutes = Minutes ?? 0;
            int seconds = Seconds ?? 0;
            int fractionalSeconds = FractionalSeconds ?? 0;
            Offset offset = Offset.Create(hours, minutes, seconds, fractionalSeconds, IsNegative);
            return ParseResult<Offset>.ForValue(offset);
        }
    }
}