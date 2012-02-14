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

using System;
using System.IO;
using NUnit.Framework;
using NodaTime;
using NodaTime.TimeZones;
using NodaTime.ZoneInfoCompiler;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace ZoneInfoCompiler.Test.Tzdb
{
    ///<summary>
    ///  This is a test class for containing all of the TzdbZoneInfoParser unit tests.
    ///</summary>
    [TestFixture]
    public class TzdbZoneInfoParserTest
    {
        private static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        private BufferLog Log { get; set; }
        private TzdbZoneInfoParser Parser { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            Log = new BufferLog();
            Parser = new TzdbZoneInfoParser(Log);
        }

        private static Offset ToOffset(int hours, int minutes, int seconds, int fractions)
        {
            return Offset.FromMilliseconds((((((hours * 60) + minutes) * 60) + seconds) * 1000) + fractions);
        }

        private static void ValidateCounts(TzdbDatabase database, int ruleSets, int zoneLists, int links)
        {
            Assert.AreEqual(ruleSets, database.Rules.Count, "Rules");
            Assert.AreEqual(zoneLists, database.Zones.Count, "Zones");
            Assert.AreEqual(links, database.Aliases.Count, "Links");
        }

        /* ############################################################################### */

        [Test]
        public void ParseDateTimeOfYear_emptyString()
        {
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingAt_invalidForRule()
        {
            const string text = "Mar lastSun";
            var tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingOn_invalidForRule()
        {
            const string text = "Mar";
            var tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingAt_validForZone()
        {
            const string text = "Mar lastSun";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, false);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int) IsoDayOfWeek.Sunday, false, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_missingOn_validForZone()
        {
            const string text = "Mar";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, false);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 1, 0, false, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onAfter()
        {
            const string text = "Mar Tue>=14 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 14, (int)IsoDayOfWeek.Tuesday, true, ToOffset(2, 0, 0, 0));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onBefore()
        {
            const string text = "Mar Tue<=14 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 14, (int)IsoDayOfWeek.Tuesday, false, ToOffset(2, 0, 0, 0));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onLast()
        {
            const string text = "Mar lastTue 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int)IsoDayOfWeek.Tuesday, false, ToOffset(2, 0, 0, 0));
            Assert.AreEqual(expected, actual);
        }

        /* ############################################################################### */

        [Test]
        public void ParseLine_comment()
        {
            const string line = "# Comment";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_commentWithLeadingWhitespace()
        {
            const string line = "   # Comment";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_emptyString()
        {
            var database = new TzdbDatabase("version");
            Parser.ParseLine(string.Empty, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_link()
        {
            const string line = "Link from to";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 1);
        }

        [Test]
        public void ParseLine_whiteSpace()
        {
            const string line = "    \t\t\n";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_zone()
        {
            const string line = "Zone PST 2:00 US P%sT";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void ParseLine_zonePlus()
        {
            const string line = "Zone PST 2:00 US P%sT";
            var database = new TzdbDatabase("version");
            Parser.ParseLine(line, database);

            const string line2 = "  3:00 US P%sT";
            Parser.ParseLine(line2, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        /* ############################################################################### */

        [Test]
        public void ParseLink_emptyString_exception()
        {
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseLink(tokens));
        }

        [Test]
        public void ParseLink_simple()
        {
            var tokens = Tokens.Tokenize("from to");
            var actual = Parser.ParseLink(tokens);
            var expected = new ZoneAlias("from", "to");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseLink_tooFewWords_exception()
        {
            var tokens = Tokens.Tokenize("from");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseLink(tokens));
        }

        [Test]
        public void ParseMonth_emptyString_default()
        {
            Assert.AreEqual(0, TzdbZoneInfoParser.ParseMonth(string.Empty));
        }

        [Test]
        public void ParseMonth_invalidMonth_default()
        {
            const string month = "Able";
            Assert.AreEqual(0, TzdbZoneInfoParser.ParseMonth(month));
        }

        [Test]
        public void ParseMonth_months()
        {
            for (int i = 0; i < MonthNames.Length; i++)
            {
                var month = MonthNames[i];
                Assert.AreEqual(i + 1, TzdbZoneInfoParser.ParseMonth(month));
            }
        }

        [Test]
        public void ParseMonth_nullArgument_default()
        {
            string month = null;
            Assert.AreEqual(0, TzdbZoneInfoParser.ParseMonth(month));
        }

        [Test]
        public void ParseZone_badOffset_exception()
        {
            var tokens = Tokens.Tokenize("asd US P%sT 1969 Mar 23 14:53:27.856s");
            Assert.Throws(typeof(FormatException), () => Parser.ParseZone(string.Empty, tokens));
        }

        /* ############################################################################### */

        [Test]
        public void ParseZone_emptyString_exception()
        {
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_optionalRule()
        {
            var tokens = Tokens.Tokenize("2:00 - P%sT");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), null, "P%sT", int.MaxValue, ZoneYearOffset.StartOfYear);
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_simple()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", int.MaxValue, ZoneYearOffset.StartOfYear);
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_tooFewWords1_exception()
        {
            var tokens = Tokens.Tokenize("2:00 US");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_tooFewWords2_exception()
        {
            var tokens = Tokens.Tokenize("2:00");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYear()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, false, Offset.Zero));
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDay()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, 23, 0, false, Offset.Zero));
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTime()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, 23, 0, false, ToOffset(14, 53, 27, 856)));
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTimeZone()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856s");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Standard, 3, 23, 0, false, ToOffset(14, 53, 27, 856)));
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withDayOfWeek()
        {
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar lastSun");
            var expected = new Zone(string.Empty, ToOffset(2, 0, 0, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int) IsoDayOfWeek.Sunday, false, Offset.Zero));
            Assert.AreEqual(expected, Parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void Parse_emptyStream()
        {
            var reader = new StringReader(string.Empty);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void Parse_threeLines()
        {
            const string text = "# A comment\n" + "Zone PST 2:00 US P%sT\n" + "         3:00 -  P%sT\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_threeLinesWithComment()
        {
            const string text = "# A comment\n" + "Zone PST 2:00 US P%sT # An end of line comment\n" + "         3:00 -  P%sT\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_twoLines()
        {
            const string text = "# A comment\n" + "Zone PST 2:00 US P%sT\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_twoLinks()
        {
            const string text = "# First line must be a comment\n" + "Link from to\n" + "Link target source\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 0, 2);
        }

        [Test]
        public void Parse_twoZones()
        {
            const string text =
                "# A comment\n" + "Zone PST 2:00 US P%sT # An end of line comment\n" + "         3:00 -  P%sT\n" + "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" + "         3:00 -  E%sT\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 2, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set " + database.Zones[0].Name);
            Assert.AreEqual(3, database.Zones[1].Count, "Zones in set " + database.Zones[1].Name);
        }

        [Test]
        public void Parse_twoZonesTwoRule()
        {
            const string text =
                "# A comment\n" + "Rule US 1987 2006 - Apr Sun>=1 2:00 1:00 D\n" + "Rule US 2007 max  - Mar Sun>=8 2:00 1:00 D\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" + "         3:00 -  P%sT\n" + "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" + "         3:00 -  E%sT\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);
            ValidateCounts(database, 1, 2, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set " + database.Zones[0].Name);
            Assert.AreEqual(3, database.Zones[1].Count, "Zones in set " + database.Zones[1].Name);
        }

        /* ############################################################################### */

        public void Parse_2400_FromDay_AtLeast_Sunday()
        {
            const string text = "Apr Sun>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = Parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.Next(Instant.FromUtc(2012, 1, 1, 0, 0), Offset.Zero, Offset.Zero);
            var expected = Instant.FromUtc(2012, 4, 2, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay_AtMost_Sunday()
        {
            const string text = "Apr Sun<=7  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = Parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.Next(Instant.FromUtc(2012, 1, 1, 0, 0), Offset.Zero, Offset.Zero);
            var expected = Instant.FromUtc(2012, 4, 2, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        public void Parse_2400_FromDay_AtLeast_Wednesday()
        {
            const string text = "Apr Wed>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = Parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.Next(Instant.FromUtc(2012, 1, 1, 0, 0), Offset.Zero, Offset.Zero);
            var expected = Instant.FromUtc(2012, 4, 5, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay_AtMost_Wednesday()
        {
            const string text = "Apr Wed<=14  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = Parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.Next(Instant.FromUtc(2012, 1, 1, 0, 0), Offset.Zero, Offset.Zero);
            var expected = Instant.FromUtc(2012, 4, 12, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay()
        {
            const string text = "Apr Sun>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 4, 1, (int)IsoDayOfWeek.Sunday, true, ToOffset(0, 0, 0, 0), true);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_Last()
        {
            const string text = "Mar lastSun 24:00";
            var tokens = Tokens.Tokenize(text);
            var actual = Parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int)IsoDayOfWeek.Sunday, false, ToOffset(0, 0, 0, 0), true);
            Assert.AreEqual(expected, actual);
        }

        /* ############################################################################### */

        [Test]
        public void Parse_Fixed_Eastern()
        {
            const string text = "# A comment\n" + "Zone\tEtc/GMT-9\t9\t-\tGMT-9\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);

            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
            var zone = database.Zones[0][0];
            Assert.AreEqual(Offset.FromHours(9), zone.Offset);
            Assert.IsNull(zone.Rules);
            Assert.AreEqual(int.MaxValue, zone.UntilYear);
        }

        [Test]
        public void Parse_Fixed_Western()
        {
            const string text = "# A comment\n" + "Zone\tEtc/GMT+9\t-9\t-\tGMT+9\n";
            var reader = new StringReader(text);
            var database = new TzdbDatabase("version");
            Parser.Parse(reader, database);

            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
            var zone = database.Zones[0][0];
            Assert.AreEqual(Offset.FromHours(-9), zone.Offset);
            Assert.IsNull(zone.Rules);
            Assert.AreEqual(int.MaxValue, zone.UntilYear);
        }
    }
}
