#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
namespace NodaTime.Format
{
    /// <summary>
    /// Factory that creates instances of PeriodFormatter.    
    /// </summary>
    /// <remarks>
    /// <para>
    /// Period formatting is performed by the <see cref="PeriodFormatter"/> class.
    /// Three classes provide factory methods to create formatters, and this is one.
    /// The others are <see cref="PeriodFormatterBuilder"/> and <see cref="ISOPeriodFormatterFactory"/>.
    /// </para>
    /// <para>
    /// PeriodFormatterFactory is thread-safe and immutable, and the formatters it returns
    /// are as well.
    /// </para>
    /// <para>
    /// TODO: Consider renaming this to PeriodFormats.
    /// </para>
    /// </remarks>
    public static class PeriodFormatterFactory
    {
        private static readonly PeriodFormatter englishWords = BuildEnglishWordsFormatter();

        /// <summary>
        /// Gets the default PeriodFormatter.
        /// </summary>
        /// <remarks>
        /// This currently returns a word based formatter using English only.
        /// Hopefully a future release will support localized period formatting.
        /// </remarks>
        public static PeriodFormatter Default { get { return englishWords; } }

        private static PeriodFormatter BuildEnglishWordsFormatter()
        {
            var variants = new[] { " ", ",", ",and ", ", and " };
            return new PeriodFormatterBuilder()
                .AppendYears().AppendSuffix(" year", " years")
                .AppendSeparator(", ", " and ", variants)
                .AppendMonths().AppendSuffix(" month", " months")
                .AppendSeparator(", ", " and ", variants)
                .AppendWeeks().AppendSuffix(" week", " weeks")
                .AppendSeparator(", ", " and ", variants)
                .AppendDays().AppendSuffix(" day", " days")
                .AppendSeparator(", ", " and ", variants)
                .AppendHours().AppendSuffix(" hour", " hours")
                .AppendSeparator(", ", " and ", variants)
                .AppendMinutes().AppendSuffix(" minute", " minutes")
                .AppendSeparator(", ", " and ", variants)
                .AppendSeconds().AppendSuffix(" second", " seconds")
                .AppendSeparator(", ", " and ", variants)
                .AppendMillis().AppendSuffix(" millisecond", " milliseconds")
                .ToFormatter();
        }
    }
}
