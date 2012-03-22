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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class InstantPatternTest : PatternTestBase<Instant>
    {
        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(Instant).FullName}},
            new Data { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(Instant).FullName } },
            new Data { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(Instant).FullName } },
            // Just a few - these are taken from other tests
            new Data { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "ffffffff", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'f', 7 } },
            new Data { Pattern = "FFFFFFFF", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'F', 7 } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Text = "rubbish", Pattern = "yyyyMMddTHH:mm:ss", Message = Messages.Parse_MismatchedNumber, Parameters = { "yyyy" } },
            new Data { Text = "17 6", Pattern = "HH h", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 'h', typeof(LocalTime).FullName}},
            new Data { Text = "17 AM", Pattern = "HH tt", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 't', typeof(LocalTime).FullName}},
        };

        internal static Data[] ParseOnlyData = {
        };

        internal static Data[] FormatOnlyData = {
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-ddTHH:mm:ss" },
            new Data(2012, 4, 28, 0, 0, 0) { Text = "2012 avr. 28", Pattern = "yyyy MMM dd", Culture = Cultures.FrFr }
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="LocalTime" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Instant>
        {
            protected override Instant DefaultTemplate
            {
                get { return Instant.UnixEpoch; }
            }

            public Data(Instant value)
                : base(value)
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second)
                : this(Instant.FromUtc(year, month, day, hour, minute, second))
            {
            }

            public Data()
                : this(Instant.UnixEpoch)
            {
            }

            internal override IPattern<Instant> CreatePattern()
            {
                return InstantPattern.CreateWithInvariantInfo(Pattern)
                    .WithCulture(Culture);
            }
        }
    }
}
