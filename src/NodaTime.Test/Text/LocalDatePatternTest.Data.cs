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
	public partial class LocalDatePatternTest
	{
        public static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        public static readonly CultureInfo EnUs = new CultureInfo("en-US");
        public static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        public static readonly CultureInfo FrCa = new CultureInfo("fr-CA");
        public static readonly CultureInfo ItIt = new CultureInfo("it-IT");

        // Used by tests via reflection - do not remove!
        private static readonly IEnumerable<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(LocalDate).FullName }},
            new Data { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(LocalDate).FullName } },
            new Data { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(LocalDate).FullName } },
            new Data { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "MMMMM", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'M', 4 } },
            new Data { Pattern = "ddddd", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'd', 4 } },
            new Data { Pattern = "H%", Message = Messages.Parse_PercentAtEndOfString },
            new Data { Pattern = "yyyyyy", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'y', 5 } },
            new Data { Pattern = "YYYYYY", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'Y', 5 } },
            new Data { Pattern = "ggg", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'g', 2 } },
            new Data { Pattern = "'qwe", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
        };

        internal static Data[] ParseFailureData = {
        };

        internal static Data[] ParseOnlyData = {
        };

        internal static Data[] FormatOnlyData = {
            // Would parse back to 2011
            new Data(1811, 7, 3) { Pattern = "yy M d", Text = "11 7 3" },
        };

        internal static Data[] FormatAndParseData = {
            new Data(2011, 10, 3) { Pattern = "yyyy/MM/dd", Text = "2011/10/03" },
            new Data(2011, 10, 3) { Pattern = "yyyy/MM/dd", Text = "2011-10-03", Culture = FrCa },
            new Data(2011, 10, 3) { Pattern = "yyyyMMdd", Text = "20111003" },
            new Data(2011, 7, 3) { Pattern = "yyy M d", Text = "2011 7 3" },
            new Data(2001, 7, 3) { Pattern = "yy M d", Text = "01 7 3" },
            new Data(2011, 7, 3) { Pattern = "yy M d", Text = "11 7 3" },
            new Data(2001, 7, 3) { Pattern = "y M d", Text = "1 7 3" },
            new Data(2011, 7, 3) { Pattern = "y M d", Text = "11 7 3" },
            new Data(1970, 10, 3) { Pattern = "MM/dd", Text = "10/03"},
            new Data(2000, 10, 3) { Pattern = "MM/dd", Text = "10/03", Template = new LocalDate(2000, 1, 1) },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        public sealed class Data : PatternTestData<LocalDate>
	    {
	        /// <summary>
	        /// Initializes a new instance of the <see cref="Data" /> class.
	        /// </summary>
	        /// <param name="value">The value.</param>
	        public Data(LocalDate value) : base(value)
	        {
	            // Default to the unix epoch in the ISO calendar
	            Template = LocalDatePattern.IsoUnixEpoch;
	        }

	        public Data(int year, int month, int day) : this(new LocalDate(year, month, day))
	        {
	        }

            public Data() : this(LocalDatePattern.IsoUnixEpoch)
            {
            }

	        internal override IPattern<LocalDate> CreatePattern()
	        {
                return LocalDatePattern.CreateWithInvariantInfo(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
	        }
	    }
	}
}
