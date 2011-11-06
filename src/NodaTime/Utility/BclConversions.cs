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

using System;

namespace NodaTime.Utility
{
    /// <summary>
    /// Conversion methods which don't naturally fit into any other types - for example, for
    /// enums which can't specify any other code. In most cases, conversions to and from BCL types
    /// are provided within the type itself - such as <see cref="LocalDateTime.ToDateTimeUnspecified"/>
    /// and <see cref="LocalDateTime.FromDateTime"/>.
    /// </summary>
    /// <remarks>
    /// Many of the methods within this class could be expressed as extension methods - but currently
    /// Noda Time always builds against .NET 2. In a future version, there may be multiple build targets,
    /// allowing these to become extension methods for the builds which use .NET 3.5 and higher.
    /// </remarks>
    public static class BclConversions
    {
        /// <summary>
        /// Converts from the Noda Time <see cref="IsoDayOfWeek"/> enum to the equivalent BCL
        /// <see cref="DayOfWeek"/> value. Other than Sunday, the BCL and ISO values are the same -
        /// but ISO 8601 defines Sunday as day 7, and the BCL defines it as day 0.
        /// </summary>
        /// <param name="isoDayOfWeek">ISO day of week value to convert.</param>
        /// <returns>The ISO day of week value equivalent to the one passed in.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="isoDayOfWeek"/> parameter
        /// is not a valid day of the week.</exception>
        public static DayOfWeek ToDayOfWeek(IsoDayOfWeek isoDayOfWeek)
        {
            if (isoDayOfWeek < IsoDayOfWeek.Monday || isoDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException("isoDayOfWeek");
            }
            return isoDayOfWeek == IsoDayOfWeek.Sunday ? DayOfWeek.Sunday : (DayOfWeek)isoDayOfWeek;
        }

        /// <summary>
        /// Converts from the BCL <see cref="DayOfWeek"/> enum to the equivalent Noda Time <see cref="IsoDayOfWeek"/> value.
        /// Other than Sunday, the BCL and ISO values are the same - but ISO 8601 defines
        /// Sunday as day 7, and the BCL defines it as day 0.
        /// </summary>
        /// <param name="dayOfWeek">ISO day of week value to convert.</param>
        /// <returns>The BCL day of week value equivalent to the one passed in.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dayOfWeek"/> parameter
        /// is not a valid day of the week.</exception>
        public static IsoDayOfWeek ToIsoDayOfWeek(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new ArgumentOutOfRangeException("dayOfWeek");
            }
            return dayOfWeek == DayOfWeek.Sunday ? IsoDayOfWeek.Sunday : (IsoDayOfWeek)dayOfWeek;
        }
    }
}
