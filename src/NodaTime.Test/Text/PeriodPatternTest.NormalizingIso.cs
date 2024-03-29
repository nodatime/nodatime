// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    public static partial class PeriodPatternTest
    {
        public class PeriodPatternNormalizingIsoTest : PatternTestBase<Period>
        {
            // Single null value to keep it from being "inconclusive"
            internal static readonly Data?[] InvalidPatternData = { null };

            internal static readonly Data[] ParseFailureData = {
                new Data { Text = "X5H", Message = TextErrorMessages.MismatchedCharacter, Parameters = { 'P' } },
                new Data { Text = "", Message = TextErrorMessages.ValueStringEmpty },
                new Data { Text = "P5J", Message = TextErrorMessages.InvalidUnitSpecifier, Parameters = { 'J' } },
                new Data { Text = "P5D10M", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'M' } },
                new Data { Text = "P6M5D6D", Message = TextErrorMessages.RepeatedUnitSpecifier, Parameters = { 'D' } },
                new Data { Text = "PT5M10H", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "P5H", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'H' } },
                new Data { Text = "PT5Y", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'Y' } },
                // Invalid in ISO.
                new Data { Text = "P", Message = TextErrorMessages.EmptyPeriod },
                new Data { Text = "PX", Message = TextErrorMessages.MissingNumber },
                new Data { Text = "P10M-", Message = TextErrorMessages.EndOfString },
                new Data { Text = "P5", Message = TextErrorMessages.EndOfString },
                new Data { Text = "PT9223372036854775808H", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "9223372036854775808", typeof(Period) } },
                new Data { Text = "PT-9223372036854775809H", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-9223372036854775809", typeof(Period) } },
                new Data { Text = "PT10000000000000000000H", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "10000000000000000000", typeof(Period) } },
                new Data { Text = "PT-10000000000000000000H", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-10000000000000000000", typeof(Period) } },
                new Data { Text = "P5.5S", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { '.' } },
                new Data { Text = "PT.5S", Message = TextErrorMessages.MissingNumber },
                new Data { Text = "PT0.5X", Message = TextErrorMessages.MismatchedCharacter, Parameters = { 'S' } },
                new Data { Text = "PT0.X", Message = TextErrorMessages.MissingNumber },
                new Data { Text = "PT5S0.5S", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { '.' } },
                new Data { Text = "PT5.", Message = TextErrorMessages.MissingNumber },
                new Data { Text = "PT5.5SX", Message = TextErrorMessages.ExpectedEndOfString }
            };

            internal static readonly Data[] ParseOnlyData = {
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT005H" },
                new Data(new PeriodBuilder { Milliseconds = 500 }) { Text = "PT0,5S" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT00000000000000000000005H" },
                new Data(new PeriodBuilder { Weeks = 5 }) { Text = "P5W" },
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
                new Data(Period.Zero) { Text = "P0D" },

                // All single values
                new Data(new PeriodBuilder { Years = 5 }) { Text = "P5Y" },
                new Data(new PeriodBuilder { Months = 5 }) { Text = "P5M" },
                new Data(new PeriodBuilder { Days = 5 }) { Text = "P5D" },
                new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT5H" },
                new Data(new PeriodBuilder { Minutes = 5 }) { Text = "PT5M" },
                new Data(new PeriodBuilder { Seconds = 5 }) { Text = "PT5S" },
                new Data(new PeriodBuilder { Milliseconds = 5 }) { Text = "PT0.005S" },
                new Data(new PeriodBuilder { Ticks = 5 }) { Text = "PT0.0000005S" },
                new Data(new PeriodBuilder { Nanoseconds = 5 }) { Text = "PT0.000000005S" },

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
                        item.StandardPattern = PeriodPattern.NormalizingIso;
                    }
                }
            }

            [Test]
            public void ParseNull() => AssertParseNull(PeriodPattern.NormalizingIso);
        }
    }
}
