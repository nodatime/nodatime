// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Test.Text;

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
            
            // Int32 max/min values +/- 1 (date units)
            new Data { Text = "P2147483648Y", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "2147483648", typeof(Period) } },
            new Data { Text = "P-2147483649Y", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-2147483649", typeof(Period) } },
            new Data { Text = "P2147483648M", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "2147483648", typeof(Period) } },
            new Data { Text = "P-2147483649M", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-2147483649", typeof(Period) } },
            new Data { Text = "P2147483648W", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "2147483648", typeof(Period) } },
            new Data { Text = "P-2147483649W", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-2147483649", typeof(Period) } },
            new Data { Text = "P2147483648D", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "2147483648", typeof(Period) } },
            new Data { Text = "P-2147483649D", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-2147483649", typeof(Period) } },

            // Int64 max/min values +/- 1 - we actually parse the value before we parse the units, so the use of an invalid unit here is deliberate to
            // make it clear that we don't need tests for all the different time units.
            new Data { Text = "PT9223372036854775808X", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "9223372036854775808", typeof(Period) } },
            new Data { Text = "PT-9223372036854775809X", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-9223372036854775809", typeof(Period) } },
            new Data { Text = "PT10000000000000000000X", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "10000000000000000000", typeof(Period) } },
            new Data { Text = "PT-10000000000000000000X", Message = TextErrorMessages.ValueOutOfRange, Parameters = { "-10000000000000000000", typeof(Period) } },

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
            // We don't normalize on parsing, only on formatting.
            new Data(new PeriodBuilder { Hours = 25 }) { Text = "PT25H" },

            // Max/min, can't be normalized but can be parsed.
            new Data(new PeriodBuilder
            {
                Years = int.MaxValue,
                Months = int.MaxValue,
                Weeks = int.MaxValue,
                Days = int.MaxValue,
                Hours = long.MaxValue,
                Minutes = long.MaxValue,
                Seconds = long.MaxValue,
                Milliseconds = 999,
                Ticks = 9999,
                Nanoseconds = 99,
            }) { Text = "P2147483647Y2147483647M2147483647W2147483647DT9223372036854775807H9223372036854775807M9223372036854775807.999999999S" },
            new Data(new PeriodBuilder
            {
                Years = int.MinValue,
                Months = int.MinValue,
                Weeks = int.MinValue,
                Days = int.MinValue,
                Hours = long.MinValue,
                Minutes = long.MinValue,
                Seconds = long.MinValue,
                Milliseconds = -999,
                Ticks = -9999,
                Nanoseconds = -99,
            }) { Text = "P-2147483648Y-2147483648M-2147483648W-2147483648DT-9223372036854775808H-9223372036854775808M-9223372036854775808.999999999S" },
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
            // Just to prove that subsecond values roundtrip (even if they're slightly odd).
            new Data(new PeriodBuilder { Days = 1, Hours = 2, Minutes = 3, Seconds = 4, Milliseconds = 123, Ticks = 4567, Nanoseconds = 89 }) { Text = "P1DT2H3M4.123456789S" },
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
