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

using System;

namespace NodaTime
{
    /// <summary>
    ///    Provides a set of
    ///    <c>static</c>
    ///    (
    ///    <c>Shared</c>
    ///    in Visual Basic) methods for querying objects that implement
    ///    <see cref="IDateTimeZone"/>
    ///    .
    /// </summary>
    public static class DateTimeZone
    {
        /// <summary>
        ///    Gets the offset from to subtract from a local time to get the UTC time.
        /// </summary>
        /// <param name="timeZone">
        ///    The time zone to use.
        /// </param>
        /// <param name="localInstant">
        ///    The local instant to get the offset of.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        ///    <para>
        ///       Note that after calculating the offset, some error may be introduced. At
        ///       offset transitions (due to DST or other historical changes), ranges of
        ///       local times may map to different UTC times.
        ///    </para>
        ///    <para>
        ///       This method will return an offset suitable for calculating an instant
        ///       after any DST gap. For example, consider a zone with a cutover
        ///       from 01:00 to 01:59:
        ///    </para>
        ///    <example>
        ///       Input: 00:00  Output: 00:00
        ///       <br/>
        ///       Input: 00:30  Output: 00:30
        ///       <br/>
        ///       Input: 01:00  Output: 02:00
        ///       <br/>
        ///       Input: 01:30  Output: 02:30
        ///       <br/>
        ///       Input: 02:00  Output: 02:00
        ///       <br/>
        ///       Input: 02:30  Output: 02:30
        ///       <br/>
        ///    </example>
        /// </remarks>
        public static Offset GetOffsetFromLocal(IDateTimeZone timeZone, LocalInstant localInstant)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone");
            }
            var instant = new Instant(localInstant.Ticks);

            // get the offset at instantLocal (first estimate)
            var offsetLocal = timeZone.GetOffsetFromUtc(instant);

            // adjust localInstant using the estimate and recalc the offset
            var offsetAdjusted = timeZone.GetOffsetFromUtc(localInstant - offsetLocal);

            // if the offsets differ, we must be near a DST boundary
            if (offsetLocal != offsetAdjusted)
            {
                // we need to ensure that time is always after the DST gap
                // this happens naturally for positive offsets, but not for negative
                if ((offsetLocal - offsetAdjusted).AsTicks() < 0)
                {
                    // if we just return offsetAdjusted then the time is pushed
                    // back before the transition, whereas it should be
                    // on or after the transition
                    var nextLocal = timeZone.NextTransition(localInstant - offsetLocal);
                    var nextAdjusted = timeZone.NextTransition(localInstant - offsetAdjusted);
                    if (nextLocal != nextAdjusted)
                    {
                        return offsetLocal;
                    }
                }
            }
            return offsetAdjusted;
        }
    }
}