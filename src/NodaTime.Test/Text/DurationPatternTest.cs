// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class DurationPatternTest : PatternTestBase<Duration>
    {
        /// <summary>
        /// Test data that can only be used to test formatting.
        /// </summary>
        internal static readonly Data[] FormatOnlyData = {
            // No sign, so we can't parse it.                                                            
            new Data(-1, 0) { Pattern = "HH:mm", Text = "01:00" },

            // Loss of nano precision
            new Data(1, 2, 3, 4, 123456789) { Pattern = "D:hh:mm:ss.ffff", Text = "1:02:03:04.1234" },
            new Data(1, 2, 3, 4, 123456789) { Pattern = "D:hh:mm:ss.FFFF", Text = "1:02:03:04.1234" },
        };

        /// <summary>
        /// Test data that can only be used to test successful parsing.
        /// </summary>
        internal static readonly Data[] ParseOnlyData = {
        };

        /// <summary>
        /// Test data for invalid patterns
        /// </summary>
        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "HH:MM", Message = Messages.Parse_MultipleCapitalDurationFields },
            new Data { Pattern = "MM mm", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'm' } },
            new Data { Pattern = "G", Message = Messages.Parse_UnknownStandardFormat, Parameters = { 'G', typeof(Duration) } },
        };

        /// <summary>
        /// Tests for parsing failures (of values)
        /// </summary>
        internal static readonly Data[] ParseFailureData = {
            new Data(Duration.Zero) { Pattern = "H:mm", Text = "1:60", Message = Messages.Parse_FieldValueOutOfRange, Parameters = { 60, 'm', typeof(Duration) }},
            // Total field values out of range
            new Data(Duration.MinValue) { Pattern = "-D:hh:mm:ss.fffffffff", Text = "16777217:00:00:00.000000000",
                Message = Messages.Parse_FieldValueOutOfRange, Parameters = { "16777217", 'D', typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-H:mm:ss.fffffffff", Text = "402653185:00:00.000000000",
                Message = Messages.Parse_FieldValueOutOfRange, Parameters = { "402653185", 'H', typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-M:ss.fffffffff", Text = "24159191041:00.000000000",
                Message = Messages.Parse_FieldValueOutOfRange, Parameters = { "24159191041", 'M', typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-S.fffffffff", Text = "1449551462401.000000000",
                Message = Messages.Parse_FieldValueOutOfRange, Parameters = { "1449551462401", 'S', typeof(Duration) } },

            // Each field in range, but overall result out of range
            new Data(Duration.MinValue) { Pattern = "-D:hh:mm:ss.fffffffff", Text = "-16777216:00:00:00.000000001",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MaxValue) { Pattern = "-D:hh:mm:ss.fffffffff", Text = "16777216:00:00:00.000000000",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-H:mm:ss.fffffffff", Text = "-402653184:00:00.000000001",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-H:mm:ss.fffffffff", Text = "402653184:00:00.000000000",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-M:ss.fffffffff", Text = "-24159191040:00.000000001",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-M:ss.fffffffff", Text = "24159191040:00.000000000",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-S.fffffffff", Text = "-1449551462400.000000001",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
            new Data(Duration.MinValue) { Pattern = "-S.fffffffff", Text = "1449551462400.000000000",
                Message = Messages.Parse_OverallValueOutOfRange, Parameters = { typeof(Duration) } },
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
            new Data(1, 2) { Pattern = "+HH:mm", Text = "+01:02" },
            new Data(-1, -2) { Pattern = "+HH:mm", Text = "-01:02" },
            new Data(1, 2) { Pattern = "-HH:mm", Text = "01:02" },
            new Data(-1, -2) { Pattern = "-HH:mm", Text = "-01:02" },
                     
            new Data(26, 3) { Pattern = "D:h:m", Text = "1:2:3" },
            new Data(26, 3) { Pattern = "DD:hh:mm", Text = "01:02:03" },
            new Data(242, 3) { Pattern = "D:hh:mm", Text = "10:02:03" },

            new Data(2, 3) { Pattern = "H:mm", Text = "2:03" },
            new Data(2, 3) { Pattern = "HH:mm", Text = "02:03" },
            new Data(26, 3) { Pattern = "HH:mm", Text = "26:03" },
            new Data(260, 3) { Pattern = "HH:mm", Text = "260:03" },

            new Data(2, 3, 4) { Pattern = "H:mm:ss", Text = "2:03:04" },

            new Data(1, 2, 3, 4, 123456789) { Pattern = "D:hh:mm:ss.fffffffff", Text = "1:02:03:04.123456789" },
            new Data(1, 2, 3, 4, 123456000) { Pattern = "D:hh:mm:ss.fffffffff", Text = "1:02:03:04.123456000" },
            new Data(1, 2, 3, 4, 123456789) { Pattern = "D:hh:mm:ss.FFFFFFFFF", Text = "1:02:03:04.123456789" },
            new Data(1, 2, 3, 4, 123456000) { Pattern = "D:hh:mm:ss.FFFFFFFFF", Text = "1:02:03:04.123456" },
            new Data(1, 2, 3) { Pattern = "M:ss", Text = "62:03" },
            new Data(1, 2, 3) { Pattern = "MMM:ss", Text = "062:03" },

            new Data(0, 0, 1, 2, 123400000) { Pattern = "SS.FFFF", Text = "62.1234" },

            new Data(1, 2, 3, 4, 123456789) { Pattern = "D:hh:mm:ss.FFFFFFFFF", Text = "1.02.03.04.123456789", Culture = Cultures.ItIt },

            // Roundtrip pattern is invariant; redundantly specify the culture to validate that it doesn't make a difference.
            new Data(1, 2, 3, 4, 123456789) { Pattern = "o", Text = "1:02:03:04.123456789", Culture = Cultures.ItIt },
            new Data(-1, -2, -3, -4, -123456789) { Pattern = "o", Text = "-1:02:03:04.123456789", Culture = Cultures.ItIt },

            // Extremes...
            new Data(Duration.MinValue) { Pattern = "-D:hh:mm:ss.fffffffff", Text = "-16777216:00:00:00.000000000" },
            new Data(Duration.MaxValue) { Pattern = "-D:hh:mm:ss.fffffffff", Text = "16777215:23:59:59.999999999" },
            new Data(Duration.MinValue) { Pattern = "-H:mm:ss.fffffffff", Text = "-402653184:00:00.000000000" },
            new Data(Duration.MaxValue) { Pattern = "-H:mm:ss.fffffffff", Text = "402653183:59:59.999999999" },
            new Data(Duration.MinValue) { Pattern = "-M:ss.fffffffff", Text = "-24159191040:00.000000000" },
            new Data(Duration.MaxValue) { Pattern = "-M:ss.fffffffff", Text = "24159191039:59.999999999" },
            new Data(Duration.MinValue) { Pattern = "-S.fffffffff", Text = "-1449551462400.000000000" },
            new Data(Duration.MaxValue) { Pattern = "-S.fffffffff", Text = "1449551462399.999999999" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Duration" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Duration>
        {
            // Ignored anyway...
            protected override Duration DefaultTemplate
            {
                get { return Duration.Zero; }
            }

            public Data()
                : this(Duration.Zero)
            {
            }

            public Data(Duration value)
                : base(value)
            {
            }

            public Data(int hours, int minutes)
                : this(Duration.FromHours(hours) + Duration.FromMinutes(minutes))
            {
            }

            public Data(int hours, int minutes, int seconds)
                : this(Duration.FromHours(hours) + Duration.FromMinutes(minutes) + Duration.FromSeconds(seconds))
            {
            }

            public Data(int days, int hours, int minutes, int seconds, long nanoseconds)
                : this(Duration.FromHours(days * 24 + hours) + Duration.FromMinutes(minutes) + Duration.FromSeconds(seconds)
                       + Duration.FromNanoseconds(nanoseconds))
            {
            }

            internal override IPattern<Duration> CreatePattern()
            {
                return DurationPattern.Create(Pattern, Culture);
            }
        }
    }
}
