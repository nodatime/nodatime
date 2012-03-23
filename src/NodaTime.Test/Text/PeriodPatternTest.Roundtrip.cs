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
    public static partial class PeriodPatternTest
    {
        /// <summary>
        /// Tests for RoundtripPatternImpl.
        /// </summary>
        [TestFixture]
        public class PeriodPatternRoundtripTest : PatternTestBase<Period>
        {
            internal static Data[] ParseFailureData = {
                new Data { Text = "X5H", Message = Messages.Parse_MismatchedCharacter, Parameters = { 'P' } },
                new Data { Text = "", Message = Messages.Parse_ValueStringEmpty },
                new Data { Text = "PJ", Message = Messages.Parse_MissingNumber },
                new Data { Text = "P5J", Message = Messages.Parse_InvalidUnitSpecifier, Parameters = { 'J' } },
                new Data { Text = "P5D10M", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'M' } },
                new Data { Text = "P6M5D6D", Message = Messages.Parse_RepeatedUnitSpecifier, Parameters = { 'D' } },
                new Data { Text = "PT5M10H", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "P5H", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "PT5Y", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'Y' } },
                new Data { Text = "P", Message = Messages.Parse_EmptyPeriod }, new Data { Text = "PX", Message = Messages.Parse_MissingNumber },
                new Data { Text = "P10M-", Message = Messages.Parse_EndOfString }, new Data { Text = "P5", Message = Messages.Parse_EndOfString },
                new Data { Text = "P9223372036854775808H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "9223372036854775808", typeof(Period) } },
                new Data { Text = "P-9223372036854775809H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "-9223372036854775809", typeof(Period) } },
                new Data { Text = "P10000000000000000000H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "10000000000000000000", typeof(Period) } },
                new Data { Text = "P-10000000000000000000H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "-10000000000000000000", typeof(Period) } },
            };

            internal static Data[] ParseOnlyData = {
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT005H" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT00000000000000000000005H" },
            };

            // This pattern round-trips, so we can always parse what we format.
            internal static Data[] FormatOnlyData = { };

            internal static readonly Data[] FormatAndParseData = {
                // All single values                                                                
                new Data(new PeriodBuilder { Years = 5 }) { Text = "P5Y" },
                new Data(new PeriodBuilder { Months = 5 }) { Text = "P5M" },
                new Data(new PeriodBuilder { Weeks = 5 }) { Text = "P5W" },
                new Data(new PeriodBuilder { Days = 5 }) { Text = "P5D" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT5H" },
                new Data(new PeriodBuilder { Minutes = 5 }) { Text = "PT5M" },
                new Data(new PeriodBuilder { Seconds = 5 }) { Text = "PT5S" },
                new Data(new PeriodBuilder { Milliseconds = 5 }) { Text = "PT5s" },
                new Data(new PeriodBuilder { Ticks = 5 }) { Text = "PT5t" },
                
                // No normalization
                new Data(new PeriodBuilder { Hours = 25, Minutes = 90 }) { Text = "PT25H90M" },

                // Compound, negative and zero tests
                new Data(new PeriodBuilder { Years = 5, Months = 2 }) { Text = "P5Y2M" },
                new Data(new PeriodBuilder { Months = 1, Hours = 0 }) { Text = "P1MT0H" },
                new Data(new PeriodBuilder { Months = 1, Minutes = -1 }) { Text = "P1MT-1M" },
                new Data(new PeriodBuilder { Hours = 1, Minutes = -1 }) { Text = "PT1H-1M" },
                
                // Max/min
                new Data(Period.FromHours(long.MaxValue)) { Text = "PT9223372036854775807H" },
                new Data(Period.FromHours(long.MinValue)) { Text = "PT-9223372036854775808H" },
            };

            internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
            internal static IEnumerable<Data> FormatData = FormatAndParseData;
        }
    }
}
