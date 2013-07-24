// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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

            // Loss of tick precision
            new Data(1, 2, 3, 4, 1234567) { Pattern = "D:hh:mm:ss.ffff", Text = "1:02:03:04.1234" },
            new Data(1, 2, 3, 4, 1234567) { Pattern = "D:hh:mm:ss.FFFF", Text = "1:02:03:04.1234" },
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
        };

        /// <summary>
        /// Tests for parsing failures (of values)
        /// </summary>
        internal static readonly Data[] ParseFailureData = {
            new Data(Duration.Zero) { Pattern = "H:mm", Text = "1:60", Message = Messages.Parse_FieldValueOutOfRange, Parameters = { 60, 'm', typeof(Duration) }},
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

            new Data(1, 2, 3, 4, 1234567) { Pattern = "D:hh:mm:ss.fffffff", Text = "1:02:03:04.1234567" },
            new Data(1, 2, 3, 4, 1234560) { Pattern = "D:hh:mm:ss.fffffff", Text = "1:02:03:04.1234560" },
            new Data(1, 2, 3, 4, 1234567) { Pattern = "D:hh:mm:ss.FFFFFFF", Text = "1:02:03:04.1234567" },
            new Data(1, 2, 3, 4, 1234560) { Pattern = "D:hh:mm:ss.FFFFFFF", Text = "1:02:03:04.123456" },
            new Data(1, 2, 3) { Pattern = "M:ss", Text = "62:03" },
            new Data(1, 2, 3) { Pattern = "MMM:ss", Text = "062:03" },

            new Data(0, 0, 1, 2, 1234000) { Pattern = "SS.FFFF", Text = "62.1234" },

            new Data(1, 2, 3, 4, 1234567) { Pattern = "D:hh:mm:ss.FFFFFFF", Text = "1.02.03.04.1234567", Culture = Cultures.ItIt },

            // Roundtrip pattern is invariant; redundantly specify the culture to validate that it doesn't make a difference.
            new Data(1, 2, 3, 4, 1234567) { Pattern = "o", Text = "1:02:03:04.1234567", Culture = Cultures.ItIt },
            new Data(-1, -2, -3, -4, -1234567) { Pattern = "o", Text = "-1:02:03:04.1234567", Culture = Cultures.ItIt },

            // Extremes...
            new Data(Duration.FromTicks(long.MaxValue)) { Pattern = "-D:hh:mm:ss.FFFFFFF", Text = "10675199:02:48:05.4775807" },
            new Data(Duration.FromTicks(long.MinValue)) { Pattern = "-D:hh:mm:ss.FFFFFFF", Text = "-10675199:02:48:05.4775808" },
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

            public Data(int days, int hours, int minutes, int seconds, int ticks)
                : this(Duration.FromHours(days * 24 + hours) + Duration.FromMinutes(minutes) + Duration.FromSeconds(seconds)
                       + Duration.FromTicks(ticks))
            {
            }

            internal override IPattern<Duration> CreatePattern()
            {
                return DurationPattern.Create(Pattern, Culture);
            }
        }
    }
}
