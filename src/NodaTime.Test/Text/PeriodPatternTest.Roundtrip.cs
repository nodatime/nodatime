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
    /// <summary>
    /// Tests for RoundtripPatternImpl.
    /// </summary>
    public class PeriodPatternRoundtripTest : PatternTestBase<Period>
    {
        internal static readonly Data?[] InvalidPatternData = { null };

        internal static Data[] ParseFailureData = {
            new Data { Text = "X5H", Message = TextErrorMessages.MismatchedCharacter, Parameters = { 'P' } },
            new Data { Text = "", Message = TextErrorMessages.ValueStringEmpty },
            new Data { Text = "PJ", Message = TextErrorMessages.MissingNumber },
            new Data { Text = "P5J", Message = TextErrorMessages.InvalidUnitSpecifier, Parameters = { 'J' } },
            new Data { Text = "P5D10M", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'M' } },
            new Data { Text = "P6M5D6D", Message = TextErrorMessages.RepeatedUnitSpecifier, Parameters = { 'D' } },
            new Data { Text = "PT5M10H", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'H' } },
            new Data { Text = "P5H", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'H' } },
            new Data { Text = "PT5Y", Message = TextErrorMessages.MisplacedUnitSpecifier, Parameters = { 'Y' } },
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
        };

        internal static Data[] ParseOnlyData = {
            new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT005H" },
            new Data(new PeriodBuilder { Hours = 5 }) { Text = "PT00000000000000000000005H" },
            // This is invalid for ISO, but *used* to be the formatted result, so we want
            // to be able to parse it in case people are parsing previous formatting results.
            // See https://github.com/nodatime/nodatime/issues/1629
            new Data(Period.Zero) { Text = "P" },
        };

        // This pattern round-trips, so we can always parse what we format.
        internal static Data[] FormatOnlyData = { };

        internal static readonly Data[] FormatAndParseData = {
            new Data(Period.Zero) { Text = "P0D" },

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
            new Data(new PeriodBuilder { Nanoseconds = 5 }) { Text = "PT5n" },

            // No normalization
            new Data(new PeriodBuilder { Hours = 25, Minutes = 90 }) { Text = "PT25H90M" },

            // Compound, negative and zero tests
            new Data(new PeriodBuilder { Years = 5, Months = 2 }) { Text = "P5Y2M" },
            new Data(new PeriodBuilder { Months = 1, Hours = 0 }) { Text = "P1M" },
            new Data(new PeriodBuilder { Months = 1, Minutes = -1 }) { Text = "P1MT-1M" },
            new Data(new PeriodBuilder { Hours = 1, Minutes = -1 }) { Text = "PT1H-1M" },

            // Max/min
            new Data(new PeriodBuilder
            {
                Years = int.MaxValue,
                Months = int.MaxValue,
                Weeks = int.MaxValue,
                Days = int.MaxValue,
                Hours = long.MaxValue,
                Minutes = long.MaxValue,
                Seconds = long.MaxValue,
                Milliseconds = long.MaxValue,
                Ticks = long.MaxValue,
                Nanoseconds = long.MaxValue
            }) { Text = "P2147483647Y2147483647M2147483647W2147483647DT9223372036854775807H9223372036854775807M9223372036854775807S9223372036854775807s9223372036854775807t9223372036854775807n" },
            new Data(new PeriodBuilder
            {
                Years = int.MinValue,
                Months = int.MinValue,
                Weeks = int.MinValue,
                Days = int.MinValue,
                Hours = long.MinValue,
                Minutes = long.MinValue,
                Seconds = long.MinValue,
                Milliseconds = long.MinValue,
                Ticks = long.MinValue,
                Nanoseconds = long.MinValue
            }) { Text = "P-2147483648Y-2147483648M-2147483648W-2147483648DT-9223372036854775808H-9223372036854775808M-9223372036854775808S-9223372036854775808s-9223372036854775808t-9223372036854775808n" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatAndParseData;

        [Test]
        public void ParseNull() => AssertParseNull(PeriodPattern.Roundtrip);

    }
}
