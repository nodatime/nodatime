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

using NUnit.Framework;
using NodaTime.Testing.TimeZones;

namespace NodaTime.Test.TimeZones
{
    // Tests for aspects of DateTimeZone to do with converting from LocalDateTime and 
    // LocalDate to ZonedDateTime.
    public partial class DateTimeZoneTest
    {
        // Sample time zones for DateTimeZone.AtStartOfDay. I didn't want to only test midnight transitions.

        /// <summary>
        /// Local midnight at the start of the transition (June 1st) becomes 1am.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardAtMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 2, 0), Offset.ForHours(-2), Offset.ForHours(-1));

        /// <summary>
        /// Local 1am at the start of the transition (June 1st) becomes midnight.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardToMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 3, 0), Offset.ForHours(-2), Offset.ForHours(-3));

        /// <summary>
        /// Local 11.20pm at the start of the transition (May 30th) becomes 12.20am of June 1st.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardBeforeMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 1, 20), Offset.ForHours(-2), Offset.ForHours(-1));

        /// <summary>
        /// Local 12.20am at the start of the transition (June 1st) becomes 11.20pm of the previous day.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardAfterMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 2, 20), Offset.ForHours(-2), Offset.ForHours(-3));

        private static readonly LocalDate TransitionDate = new LocalDate(2000, 6, 1);

        [Test]
        public void AmbiguousStartOfDay_TransitionAtMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalInstant(2000, 6, 1, 0, 0), Offset.ForHours(-2),
                TransitionBackwardToMidnightZone.ToIsoChronology());
            var actual = TransitionBackwardToMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AmbiguousStartOfDay_TransitionAfterMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalInstant(2000, 6, 1, 0, 0), Offset.ForHours(-2),
                TransitionBackwardAfterMidnightZone.ToIsoChronology());
            var actual = TransitionBackwardAfterMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionAtMidnight()
        {
            // 1am because of the skip
            var expected = new ZonedDateTime(new LocalInstant(2000, 6, 1, 1, 0), Offset.ForHours(-1),
                TransitionForwardAtMidnightZone.ToIsoChronology());
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionBeforeMidnight()
        {
            // 12.20am because of the skip
            var expected = new ZonedDateTime(new LocalInstant(2000, 6, 1, 0, 20), Offset.ForHours(-1),
                TransitionForwardBeforeMidnightZone.ToIsoChronology());
            var actual = TransitionForwardBeforeMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnambiguousStartOfDay()
        {
            // Just a simple midnight in March.
            var expected = new ZonedDateTime(new LocalInstant(2000, 3, 1, 0, 0), Offset.ForHours(-2),
                TransitionForwardAtMidnightZone.ToIsoChronology());
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(new LocalDate(2000, 3, 1));
            Assert.AreEqual(expected, actual);
        }
    }
}
