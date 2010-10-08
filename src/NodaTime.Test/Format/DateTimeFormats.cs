#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.Globalization;
using NodaTime.Format;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public class DateTimeFormatsTest
    {
        #region Zones
        private static readonly DateTimeZone UTC = DateTimeZone.Utc;
        private static DateTimeZone London = DateTimeZones.ForId("Europe/London");
        private static DateTimeZone Paris = DateTimeZones.ForId("Europe/Paris");
        #endregion

        #region Cultures
        private static readonly IFormatProvider EnUs = CultureInfo.GetCultureInfo("en-US"); //English (United States) 
        private static readonly IFormatProvider PortuBrazil = CultureInfo.GetCultureInfo("pt-BR"); //Portuguese (Brazil) 
        private static readonly IFormatProvider CroCro = CultureInfo.GetCultureInfo("hr-HR"); //Croatian (Croatia) 
        #endregion

        private object[] StandardPatternTestData = {
            new TestCaseData('d', (IFormatProvider)null, new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC),
                             new DateTimeOffset(2004, 6, 9, 10, 20, 30, 40, TimeSpan.Zero)),
            new TestCaseData('d', EnUs, new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), new DateTimeOffset(2004, 6, 9, 10, 20, 30, 40, TimeSpan.Zero)),
            new TestCaseData('d', PortuBrazil, new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), new DateTimeOffset(2004, 6, 9, 10, 20, 30, 40, TimeSpan.Zero))
            , new TestCaseData('d', CroCro, new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, UTC), new DateTimeOffset(2004, 6, 9, 10, 20, 30, 40, TimeSpan.Zero)),
        };

        [Test]
        [TestCaseSource("StandardPatternTestData")]
        public void StandardPattern_OutputsTheSameStringAsBCL(char standardPattern, IFormatProvider provider, ZonedDateTime nDateTime, DateTimeOffset sDateTime)
        {
            var nodaOutput = DateTimeFormats.ForStandardPattern(standardPattern).WithProvider(provider).Print(nDateTime);
            var bclOutput = sDateTime.ToString(standardPattern.ToString(), provider);

            Assert.That(nodaOutput, Is.EqualTo(bclOutput));
        }
    }
}