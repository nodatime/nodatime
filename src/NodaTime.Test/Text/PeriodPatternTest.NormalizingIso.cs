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
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;
using System.Linq;

namespace NodaTime.Test.Text
{
    public static partial class PeriodPatternTest
    {
        [TestFixture]
        public class PeriodPatternNormalizingIsoTest : PatternTestBase<Period>
        {
            internal static readonly Data[] InvalidPatternData = {};

            internal static readonly Data[] ParseFailureData = {
                new Data { Text = "X5H", Message = Messages.Parse_MismatchedCharacter, Parameters = { 'P' } },
                new Data { Text = "", Message = Messages.Parse_ValueStringEmpty },
                new Data { Text = "P5J", Message = Messages.Parse_InvalidUnitSpecifier, Parameters = { 'J' } },
                new Data { Text = "P5D10M", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'M' } },
                new Data { Text = "P6M5D6D", Message = Messages.Parse_RepeatedUnitSpecifier, Parameters = { 'D' } },
                new Data { Text = "PT5M10H", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "P5H", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "PT5Y", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { 'Y' } },
                new Data { Text = "P", Message = Messages.Parse_EmptyPeriod }, new Data { Text = "PX", Message = Messages.Parse_MissingNumber },
                new Data { Text = "P10M-", Message = Messages.Parse_EndOfString }, new Data { Text = "P5", Message = Messages.Parse_EndOfString },
                new Data { Text = "PT9223372036854775808H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "9223372036854775808", typeof(Period) } },
                new Data { Text = "PT-9223372036854775809H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "-9223372036854775809", typeof(Period) } },
                new Data { Text = "PT10000000000000000000H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "10000000000000000000", typeof(Period) } },
                new Data { Text = "PT-10000000000000000000H", Message = Messages.Parse_ValueOutOfRange, Parameters = { "-10000000000000000000", typeof(Period) } },
                new Data { Text = "P5.5S", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { '.' } },
                new Data { Text = "PT.5S", Message = Messages.Parse_MissingNumber },
                new Data { Text = "PT0.5X", Message = Messages.Parse_MismatchedCharacter, Parameters = { 'S' } },
                new Data { Text = "PT5S0.5S", Message = Messages.Parse_MisplacedUnitSpecifier, Parameters = { '.' } },
                new Data { Text = "PT5.", Message = Messages.Parse_MissingNumber },
                new Data { Text = "PT5.5SX", Message = Messages.Parse_ExpectedEndOfString }
            };

            internal static readonly Data[] ParseOnlyData = {
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT005H" },
                new Data(new PeriodBuilder { Milliseconds = 500 }) { Text = "PT0,5S" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT00000000000000000000005H" },
            };

            // Only a small amount of testing here - it's around normalization, which is
            // unit tested more thoroughly elsewhere.
            internal static readonly Data[] FormatOnlyData = {
                new Data(new PeriodBuilder { Hours = 25, Minutes = 90 }) { Text = "P1D2H30M" },
                new Data(new PeriodBuilder { Ticks = 12345678 }) { Text = "P1.2345678S" },
                new Data(new PeriodBuilder { Hours = 1, Minutes = -1 }) { Text = "PT59M" },
                new Data(new PeriodBuilder { Hours = -1, Minutes = 1 }) { Text = "PT-59M" },
                new Data(new PeriodBuilder { Weeks = 5 }) { Text = "P35D" },
            };

            internal static readonly Data[] FormatAndParseData = {
                // All single values                                                                
                new Data(new PeriodBuilder { Years = 5 }) { Text = "P5Y" },
                new Data(new PeriodBuilder { Months = 5 }) { Text = "P5M" },
                new Data(new PeriodBuilder { Days = 5 }) { Text = "P5D" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT5H" },
                new Data(new PeriodBuilder { Minutes = 5 }) { Text = "PT5M" },
                new Data(new PeriodBuilder { Seconds = 5 }) { Text = "PT5S" },
                new Data(new PeriodBuilder { Milliseconds = 5 }) { Text = "PT0.005S" },
                new Data(new PeriodBuilder { Ticks = 5 }) { Text = "PT0.0000005S" },
                
                // Compound, negative and zero tests
                new Data(new PeriodBuilder { Years = 5, Months = 2 }) { Text = "P5Y2M" },
                new Data(new PeriodBuilder { Months = 1, Hours = 0 }) { Text = "P1M" },
                new Data(new PeriodBuilder { Months = 1, Minutes = -1 }) { Text = "P1MT-1M" },
                new Data(new PeriodBuilder { Seconds = 1, Milliseconds = 320 }) { Text = "PT1.32S" },
                new Data(new PeriodBuilder { Seconds = -1 }) { Text = "PT-1S" },
                new Data(new PeriodBuilder { Seconds = -1, Milliseconds = -320 }) { Text = "PT-1.32S" },
                new Data(new PeriodBuilder { Milliseconds = -320 }) { Text = "PT-0.32S" },
            };

            internal static readonly IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
            internal static readonly IEnumerable<Data> FormatData = FormatAndParseData;

            // Go over all our sequences and change the pattern to use. This is ugly,
            // but it beats specifying it on each line.
            static PeriodPatternNormalizingIsoTest()
            {
                foreach (var sequence in new[] { ParseFailureData, ParseData, FormatData })
                {
                    foreach (var item in sequence)
                    {
                        item.StandardPattern = PeriodPattern.NormalizingIsoPattern;
                    }
                }
            }
        }
    }
}
