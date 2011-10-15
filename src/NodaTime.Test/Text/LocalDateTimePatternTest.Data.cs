#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using System.Globalization;
using System.Linq;
using NodaTime.Text;
using NodaTime.Properties;
   
namespace NodaTime.Test.Text
{
    public partial class LocalDateTimePatternTest
    {
        public static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        public static readonly CultureInfo EnUs = new CultureInfo("en-US");
        public static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        public static readonly CultureInfo FrCa = new CultureInfo("fr-CA");
        public static readonly CultureInfo ItIt = new CultureInfo("it-IT");

        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns.
        private static readonly LocalDateTime MsdnStandardExample = new LocalDateTime(2009, 06, 15, 13, 45, 30, 90);
        private static readonly LocalDateTime MsdnStandardExampleNoMillis = new LocalDateTime(2009, 06, 15, 13, 45, 30);
        private static readonly LocalDateTime MsdnStandardExampleNoSeconds = new LocalDateTime(2009, 06, 15, 13, 45);

        internal static readonly Data[] InvalidPatternData = {
        };

        internal static Data[] ParseFailureData = {
        };

        internal static Data[] ParseOnlyData = {
        };

        internal static Data[] FormatOnlyData = {
        };

        internal static Data[] FormatAndParseData = {
            // Standard patterns (US)
            // Full date/time (short time)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "f", Text = "Monday, June 15, 2009 1:45 PM", Culture = EnUs },
            // Full date/time (long time)
            new Data(MsdnStandardExampleNoMillis) { Pattern = "F", Text = "Monday, June 15, 2009 1:45:30 PM", Culture = EnUs },
            // General date/time (short time)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "g", Text = "6/15/2009 1:45 PM", Culture = EnUs },
            // General date/time (longtime)
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "6/15/2009 1:45:30 PM", Culture = EnUs },
            // Round-trip (o and O - same effect)
            new Data(MsdnStandardExample) { Pattern = "o", Text = "2009-06-15T13:45:30.0900000", Culture = EnUs },
            new Data(MsdnStandardExample) { Pattern = "O", Text = "2009-06-15T13:45:30.0900000", Culture = EnUs },
            // Note: No RFC1123, as that requires a time zone.
            // Sortable / ISO8601
            new Data(MsdnStandardExampleNoMillis) { Pattern = "s", Text = "2009-06-15T13:45:30", Culture = EnUs },

            // Standard patterns (French)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "f", Text = "lundi 15 juin 2009 13:45", Culture = FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "F", Text = "lundi 15 juin 2009 13:45:30", Culture = FrFr },
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "g", Text = "15/06/2009 13:45", Culture = FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "15/06/2009 13:45:30", Culture = FrFr },
            // Culture has no impact on round-trip or sortable formats
            new Data(MsdnStandardExample) { Pattern = "o", Text = "2009-06-15T13:45:30.0900000", Culture = FrFr },
            new Data(MsdnStandardExample) { Pattern = "O", Text = "2009-06-15T13:45:30.0900000", Culture = FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "s", Text = "2009-06-15T13:45:30", Culture = FrFr },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        public sealed class Data : PatternTestData<LocalDateTime>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(LocalDateTime value) : base(value)
            {
                // Default to the start of the year 2000.
                Template = LocalDateTimePattern.DefaultTemplateValue;
            }

            public Data(int year, int month, int day) : this(new LocalDateTime(year, month, day, 0, 0))
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second)
                : this(new LocalDateTime(year, month, day, hour, minute, second))
            {
            }

            public Data(LocalDate date, LocalTime time)
                : this(date + time)
            {
            }

            public Data()
                : this(LocalDateTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<LocalDateTime> CreatePattern()
            {
                return LocalDateTimePattern.CreateWithInvariantInfo(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
            }
        }
    }
}
