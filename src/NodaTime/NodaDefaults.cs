#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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

using NodaTime.Periods;

namespace NodaTime
{
    /// <summary>
    /// Original name: DateTimeUtils.
    /// Contains the methods to check for null values of some NodaTime classes
    /// and to return valid not-null values.
    /// TODO: Consider removing this. I (Jon) believe that Joda Time made a mistake in
    /// allowing nulls in so many places. We can provide defaults through overloading,
    /// but when a value is available it shouldn't be null. To be discussed.
    /// </summary>
    internal static class NodaDefaults
    {
        /// <summary>
        /// Gets the period type handling null.
        /// </summary>
        /// <param name="periodType">The period type to use, null means the standard period type</param>
        /// <returns>The type to use, never null</returns>
        /// <remarks>
        /// If the period type is <code>null</code>, <see cref="PeriodType.Standard"/>
        /// will be returned. Otherwise, the type specified is returned.
        /// </remarks>
        internal static PeriodType CheckPeriodType(PeriodType periodType)
        {
            return periodType ?? PeriodType.Standard;
        }
    }
}