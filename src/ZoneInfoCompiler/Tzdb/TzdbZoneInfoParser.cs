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
using System;
using System.Globalization;
using System.IO;
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Provides a parser for TZDB time zone description files.
    /// </summary>
    internal class TzdbZoneInfoParser
    {
        /// <summary>
        /// The keyword that specifies the line defines an alias link.
        /// </summary>
        private const string KeywordLink = "Link";

        /// <summary>
        /// The keyword that specifies the line defines a daylight savings rule.
        /// </summary>
        private const string KeywordRule = "Rule";

        /// <summary>
        /// The keyword that specifies the line defines a time zone. 
        /// </summary>
        private const string KeywordZone = "Zone";

        /// <summary>
        /// The months of the year names as they appear in the TZDB zone files. They are
        /// always the short name in US English. Extra blank name at the beginning helps
        /// to make the indexes to come out right.
        /// </summary>
        public static readonly string[] Months = { 
                                             "",
                                             "Jan",
                                             "Feb",
                                             "Mar",
                                             "Apr",
                                             "May",
                                             "Jun",
                                             "Jul",
                                             "Aug",
                                             "Sep",
                                             "Oct",
                                             "Nov",
                                             "Dec"
                                         };

        /// <summary>
        /// The days of the week names as they appear in the TZDB zone files. They are
        /// always the short name in US English.
        /// </summary>
        public static readonly string[] DaysOfWeek = {
                                                         "",
                                                         "Mon",
                                                         "Tue",
                                                         "Wed",
                                                         "Thu",
                                                         "Fri",
                                                         "Sat",
                                                         "Sun"
                                                     };

        /// <summary>
        /// Gets or sets the log to use for logging messages.
        /// </summary>
        /// <value>The log object.</value>
        internal ILog Log { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbZoneInfoParser"/> class.
        /// </summary>
        /// <param name="log">The log to use for logging messages.</param>
        internal TzdbZoneInfoParser(ILog log)
        {
            Log = log;
        }

        /// <summary>
        /// Parses the TZDB time zone info file from the given stream and merges its information
        /// with the given database. The stream is not closed or disposed.
        /// </summary>
        /// <param name="input">The stream input to parse.</param>
        /// <param name="database">The database to fill.</param>
        public void Parse(Stream input, TzdbDatabase database)
        {
            Parse(new StreamReader(input, true), database);
        }

        /// <summary>
        /// Parses the TZDB time zone info file from the given reader and merges its information
        /// with the given database. The reader is not closed or disposed.
        /// </summary>
        /// <param name="reader">The reader to read.</param>
        /// <param name="database">The database to fill.</param>
        public void Parse(TextReader reader, TzdbDatabase database)
        {
            Log.LineNumber = 1;
            try
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    try
                    {
                        if (Log.LineNumber == 1)
                        {
                            if (!line.StartsWith("# "))
                            {
                                return;
                            }
                        }
                        else
                        {
                            ParseLine(line, database);
                        }
                    }
                    catch (ParseException)
                    {
                        // Nothing to do
                    }
                    catch (Exception e)
                    {
                        Log.Error("Exception {0} occurred: {1}", e.GetType().Name, e.Message);
                        throw;
                    }
                    Log.LineNumber++;
                }
            }
            finally
            {
                Log.LineNumber = -1;
            }
        }

        /// <summary>
        /// Parses a single line of an TZDB zone info file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// TZDB files have a simple line based structure. Each line defines one item. Comments
        /// start with a hash or pound sign (#) and continue to the end of the line. Blank lines are
        /// ignored. Of the remaining there are four line types which are determined by the first
        /// keyword on the line.
        /// </para>
        /// <para>
        /// A line beginning with the keyword <c>Link</c> defines an alias between one time zone and
        /// another. Both time zones use the same definition but have different names.
        /// </para>
        /// <para>
        /// A line beginning with the keyword <c>Rule</c> defines a daylight savings time
        /// calculation rule.
        /// </para>
        /// <para>
        /// A line beginning with the keyword <c>Zone</c> defines a time zone.
        /// </para>
        /// <para>
        /// A line beginning with leading whitespace (an empty keyword) defines another part of the
        /// preceeding time zone. As many lines as necessary to define the time zone can be listed,
        /// but they must all be together and only the first line can have a name.
        /// </para>
        /// </remarks>
        /// <param name="line">The line to parse.</param>
        /// <param name="database">The database to fill.</param>
        internal void ParseLine(string line, TzdbDatabase database)
        {
            int index = line.IndexOf("#", StringComparison.Ordinal);
            if (index == 0)
            {
                return;
            }
            if (index > 0)
            {
                line = line.Substring(0, index - 1);
            }
            line = line.TrimEnd();
            if (line.Length == 0)
            {
                return;
            }
            Tokens tokens = Tokens.Tokenize(line);
            string keyword = NextString(tokens, "Keyword");
            if (keyword == KeywordRule)
            {
                ZoneRule rule = ParseRule(tokens);
                database.AddRule(rule);
            }
            else if (keyword == KeywordLink)
            {
                ZoneAlias alias = ParseLink(tokens);
                database.AddAlias(alias);
            }
            else if (keyword == KeywordZone)
            {
                string name = NextString(tokens, "Name");
                Zone zone = ParseZone(name, tokens);
                database.AddZone(zone);
            }
            else if (keyword == string.Empty)
            {
                Zone zone = ParseZone(string.Empty, tokens);
                database.AddZone(zone);
            }
            else
            {
                Error("Unexpected zone database keyword: {0}", keyword);
            }
        }

        /// <summary>
        /// Parses an alias link and returns the ZoneAlias object.
        /// </summary>
        /// <param name="tokens">The tokens to parse.</param>
        /// <returns>The ZoneAlias object.</returns>
        internal ZoneAlias ParseLink(Tokens tokens)
        {
            string existing = NextString(tokens, "Existing");
            string alias = NextString(tokens, "Alias");
            return new ZoneAlias(existing, alias);
        }

        /// <summary>
        /// Parses a daylight savings rule and returns the Rule object.
        /// </summary>
        /// <remarks>
        /// # Rule	NAME	FROM	TO	TYPE	IN	ON	AT	SAVE	LETTER/S
        /// </remarks>
        /// <param name="tokens">The tokens to parse.</param>
        /// <returns>The Rule object.</returns>
        internal ZoneRule ParseRule(Tokens tokens)
        {
            string name = NextString(tokens, "Name");
            int fromYear = NextYear(tokens, "FromYear", 0);
            int toYear = NextYear(tokens, "ToYear", fromYear);
            if (toYear < fromYear)
            {
                throw new ArgumentException("To year cannot be before the from year in a Rule: " + toYear + " < " + fromYear);
            }
            string type = NextOptional(tokens, "Type");
            ZoneYearOffset yearOffset = ParseDateTimeOfYear(tokens);
            Offset savings = NextOffset(tokens, "SaveMillis");
            string letterS = NextOptional(tokens, "LetterS");
            ZoneRecurrence recurrence = new ZoneRecurrence(name, savings, yearOffset, fromYear, toYear);
            return new ZoneRule(recurrence, letterS); ;
        }

        /// <summary>
        /// Parses the date time of year.
        /// </summary>
        /// <param name="tokens">The tokens to parse.</param>
        /// <returns>The DateTimeOfYear object.</returns>
        /// <remarks>
        /// IN ON AT
        /// </remarks>
        internal ZoneYearOffset ParseDateTimeOfYear(Tokens tokens)
        {
            TransitionMode mode = ZoneYearOffset.StartOfYear.Mode;
            int monthOfYear = ZoneYearOffset.StartOfYear.MonthOfYear;
            int dayOfMonth = ZoneYearOffset.StartOfYear.DayOfMonth;
            int dayOfWeek = ZoneYearOffset.StartOfYear.DayOfWeek;
            bool advanceDayOfWeek = ZoneYearOffset.StartOfYear.AdvanceDayOfWeek;
            Offset tickOfDay = ZoneYearOffset.StartOfYear.TickOfDay;

            monthOfYear = NextMonth(tokens, "MonthOfYear");

            String on = NextString(tokens, "When");
            if (on.StartsWith("last", StringComparison.Ordinal))
            {
                dayOfMonth = -1;
                dayOfWeek = ParseDayOfWeek(on.Substring(4));
                advanceDayOfWeek = false;
            }
            else
            {
                int index = on.IndexOf(">=", StringComparison.Ordinal);
                if (index > 0)
                {
                    dayOfMonth = Int32.Parse(on.Substring(index + 2), CultureInfo.InvariantCulture);
                    dayOfWeek = ParseDayOfWeek(on.Substring(0, index));
                    advanceDayOfWeek = true;
                }
                else
                {
                    index = on.IndexOf("<=", StringComparison.Ordinal);
                    if (index > 0)
                    {
                        dayOfMonth = Int32.Parse(on.Substring(index + 2), CultureInfo.InvariantCulture);
                        dayOfWeek = ParseDayOfWeek(on.Substring(0, index));
                        advanceDayOfWeek = false;
                    }
                    else
                    {
                        try
                        {
                            dayOfMonth = Int32.Parse(on, CultureInfo.InvariantCulture);
                            dayOfWeek = 0;
                            advanceDayOfWeek = false;
                        }
                        catch (FormatException e)
                        {
                            throw new ArgumentException("Unparsable ON token: " + on, e);
                        }
                    }
                }
            }

            string atTime = NextString(tokens, "AT");
            if (atTime != null && atTime.Length > 0)
            {
                if (Char.IsLetter(atTime[atTime.Length - 1]))
                {
                    char zoneCharacter = atTime[atTime.Length - 1];
                    mode = ZoneYearOffset.NormalizeModeCharacter(zoneCharacter);
                    atTime = atTime.Substring(0, atTime.Length - 1);
                }
                tickOfDay = ParserHelper.ParseOffset(atTime);
            }
            return new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advanceDayOfWeek, tickOfDay);
        }

        /// <summary>
        /// Parses a time zone definition and returns the Zone object.
        /// </summary>
        /// <remarks>
        /// # GMTOFF RULES FORMAT [ UntilYear [ UntilMonth [ UntilDay [ UntilTime [ ZoneCharacter ] ] ] ] ]
        /// </remarks>
        /// <param name="tokens">The tokens to parse.</param>
        /// <returns>The Zone object.</returns>
        internal Zone ParseZone(string name, Tokens tokens)
        {
            Offset offset = NextOffset(tokens, "Gmt Offset");
            string rules = NextOptional(tokens, "Rules");
            string format = NextString(tokens, "Format");
            int year = NextYear(tokens, "Until Year", Int32.MaxValue);
            int monthOfYear = NextMonth(tokens, "Until Month", NodaConstants.January);
            int dayOfMonth = NextInteger(tokens, "Until Day", 1);
            Offset tickOfDay = Offset.Zero;
            char zoneCharacter = (char)0;
            string untilTime = NextString(tokens, "Until Time", null);
            if (untilTime != null && untilTime.Length > 0)
            {
                if (Char.IsLetter(untilTime[untilTime.Length - 1]))
                {
                    zoneCharacter = untilTime[untilTime.Length - 1];
                    untilTime = untilTime.Substring(0, untilTime.Length - 1);
                }
                tickOfDay = ParserHelper.ParseOffset(untilTime);
            }
            return new Zone(name, offset, rules, format, year, monthOfYear, dayOfMonth, tickOfDay, zoneCharacter);
        }

        /// <summary>
        /// Parses the month.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The month number 1-12 or 0 if the month is not valid</returns>
        internal int ParseMonth(String text)
        {
            for (int i = 1; i < Months.Length; i++)
            {
                if (text == Months[i])
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Parses the day of week.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        internal int ParseDayOfWeek(String text)
        {
            for (int i = 1; i < DaysOfWeek.Length; i++)
            {
                if (text == DaysOfWeek[i])
                {
                    return i;
                }
            }
            throw new ArgumentException("The value is not a valid day of week: " + text);
        }

        /// <summary>
        /// Nexts the offset.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private int NextInteger(Tokens tokens, string name)
        {
            string value = NextString(tokens, name);
            return ParserHelper.ParseInteger(value);
        }

        /// <summary>
        /// Nexts the offset.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private int NextInteger(Tokens tokens, string name, int defaultValue)
        {
            int result = defaultValue;
            string text;
            if (tokens.TryNextToken(name, out text))
            {
                result = ParserHelper.ParseInteger(text, defaultValue);
            }
            return result;
        }

        /// <summary>
        /// Nexts the offset.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private Offset NextOffset(Tokens tokens, string name)
        {
            string value = NextString(tokens, name);
            return ParserHelper.ParseOffset(value);
        }

        /// <summary>
        /// Nexts the offset.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private Offset NextOffset(Tokens tokens, string name, Offset defaultValue)
        {
            Offset result = defaultValue;
            string text;
            if (tokens.TryNextToken(name, out text))
            {
                result = ParserHelper.ParseOffset(text);
            }
            return result;
        }

        /// <summary>
        /// Nexts the year.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private int NextYear(Tokens tokens, string name, int defaultValue)
        {
            int result = defaultValue;
            string text;
            if (tokens.TryNextToken(name, out text))
            {
                result = ParserHelper.ParseYear(text, defaultValue);
            }
            return result;
        }

        /// <summary>
        /// Nexts the month.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private int NextMonth(Tokens tokens, string name)
        {
            string value = NextString(tokens, name);
            int result = ParseMonth(value);
            if (result == 0)
            {
                result = NodaConstants.January;
            }
            return result;
        }

        /// <summary>
        /// Nexts the month.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private int NextMonth(Tokens tokens, string name, int defaultValue)
        {
            int result = defaultValue;
            string text;
            if (tokens.TryNextToken(name, out text))
            {
                result = ParseMonth(text);
                if (result == 0)
                {
                    result = defaultValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Nexts the optional.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string NextOptional(Tokens tokens, string name)
        {
            string value = NextString(tokens, name);
            return ParserHelper.ParseOptional(value);
        }

        /// <summary>
        /// Nexts the string.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string NextString(Tokens tokens, string name)
        {
            if (!tokens.HasNextToken)
            {
                Error("Missing zone info token {0}", name);
            }
            return tokens.NextToken(name);
        }

        /// <summary>
        /// Nexts the string.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private string NextString(Tokens tokens, string name, string defaultValue)
        {
            string result;
            if (!tokens.TryNextToken(name, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Logs the given informational message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="arguments">The optional arguments for the message.</param>
        private void Informational(string format, params object[] arguments)
        {
            Log.Info(format, arguments);
        }

        /// <summary>
        /// Logs the given warning message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="arguments">The optional arguments for the message.</param>
        private void Warning(string format, params object[] arguments)
        {
            Log.Warn(format, arguments);
        }

        /// <summary>
        /// Logs the given error message and then throws a ParseException to return to the top level.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="arguments">The optional arguments for the message.</param>
        private void Error(string format, params object[] arguments)
        {
            Log.Error(format, arguments);
            throw new ParseException();
        }

        /// <summary>
        /// Private exception to use to end the parsing of a line and return to the top level.
        /// This should NEVER propagate out of this file. Must be internal so the tests can see it.
        /// </summary>
        internal class ParseException
            : Exception
        {
        }
    }
}
