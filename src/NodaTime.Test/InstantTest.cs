#region Copyright and license information

// // Copyright 2001-2009 Stephen Colebourne
// // Copyright 2009 Jon Skeet
// // 
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// //
// //     http://www.apache.org/licenses/LICENSE-2.0
// //
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.

#endregion

using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class InstantTest
    {
        // test in 2002/03 as time zones are more well known
        // (before the late 90's they were all over the place)
        private static readonly DateTimeZone Paris = DateTimeZone.ForID("Europe/Paris");
        private static readonly DateTimeZone London = DateTimeZone.ForID("Europe/London");

        private const long Y2002Days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365;
        private const long Y2003Days = 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 +
                                       365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 +
                                       366 + 365 + 365;

        // 1970-06-09
        private const long TestTimeNow =
            (31L + 28L + 31L + 30L + 31L + 9L - 1L) * DateTimeConstants.MillisecondsPerDay;
        // 1970-04-05
        private const long TestTime1 =
            (31L + 28L + 31L + 6L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 12L * DateTimeConstants.MillisecondsPerHour
            + 24L * DateTimeConstants.MillisecondsPerMinute;

        // 1971-05-06
        private const long TestTime2 =
            (365L + 31L + 28L + 31L + 30L + 7L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 14L * DateTimeConstants.MillisecondsPerHour
            + 28L * DateTimeConstants.MillisecondsPerMinute;
    }
}