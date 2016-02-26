// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    /// <summary>
    /// Tests for TzdbZoneInfoParser.
    /// </summary>
    public class TzdbZoneInfoParserTest
    {
        private static Offset ToOffset(int hours, int minutes)
        {
            return Offset.FromHoursAndMinutes(hours, minutes);
        }

        private static void ValidateCounts(TzdbDatabase database, int ruleSets, int zoneLists, int links)
        {
            Assert.AreEqual(ruleSets, database.Rules.Count, "Rules");
            Assert.AreEqual(zoneLists, database.Zones.Count, "Zones");
            Assert.AreEqual(links, database.Aliases.Count, "Links");
        }

        [Test]
        public void ParseDateTimeOfYear_emptyString()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingAt_invalidForRule()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar lastSun";
            var tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingOn_invalidForRule()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar";
            var tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseDateTimeOfYear(tokens, true));
        }

        [Test]
        public void ParseDateTimeOfYear_missingAt_validForZone()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar lastSun";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, false);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int) IsoDayOfWeek.Sunday, false, LocalTime.Midnight);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_missingOn_validForZone()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, false);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 1, 0, false, LocalTime.Midnight);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onAfter()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar Tue>=14 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 14, (int)IsoDayOfWeek.Tuesday, true, new LocalTime(2, 0));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onBefore()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar Tue<=14 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, 14, (int)IsoDayOfWeek.Tuesday, false, new LocalTime(2, 0));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onLast()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar lastTue 2:00";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int)IsoDayOfWeek.Tuesday, false, new LocalTime(2, 0));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseLine_comment()
        {
            const string line = "# Comment";
            var database = ParseText(line);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_commentWithLeadingWhitespace()
        {
            const string line = "   # Comment";
            var database = ParseText(line);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_emptyString()
        {
            var database = ParseText("");
            ValidateCounts(database, 0, 0, 0);
        }

        // Assume that all lines work the same way - it's comment handling
        // that's important here.
        [Test]
        public void ParseLine_commentAtEndOfLine()
        {
            string line = "Link from to#Comment";
            var database = ParseText(line);
            ValidateCounts(database, 0, 0, 1);
            Assert.AreEqual("from", database.Aliases["to"]);
        }

        [Test]
        public void ParseLine_link()
        {
            const string line = "Link from to";
            var database = ParseText(line);
            ValidateCounts(database, 0, 0, 1);
        }

        [Test]
        public void ParseLine_whiteSpace()
        {
            const string line = "    \t\t\n";
            var database = ParseText(line);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_zone()
        {
            const string line = "Zone PST 2:00 US P%sT";
            var database = ParseText(line);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones.Values.Single().Count);
        }

        [Test]
        public void ParseLine_zonePlus()
        {
            string lines =
                "Zone PST 2:00 US P%sT\n" +
                "  3:00 US P%sT";
            var database = ParseText(lines);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones["PST"].Count);
        }

        [Test]
        public void ParseLink_emptyString_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseLink(tokens));
        }

        [Test]
        public void ParseLink_simple()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("from to");
            var actual = parser.ParseLink(tokens);
            var expected = Tuple.Create("from", "to");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseLink_tooFewWords_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("from");
            Assert.Throws<InvalidDataException>(() => parser.ParseLink(tokens));
        }

        [Test]
        public void ParseMonth_nullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => TzdbZoneInfoParser.ParseMonth(""));
            Assert.Throws<ArgumentException>(() => TzdbZoneInfoParser.ParseMonth(null));
        }

        [Test]
        public void ParseMonth_invalidMonth_default()
        {
            Assert.Throws<InvalidDataException>(() => TzdbZoneInfoParser.ParseMonth("Able"));
        }

        [Test]
        public void ParseMonth_shortMonthNames()
        {
            for (int i = 1; i < 12; i++)
            {
                var month = new DateTime(2000, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                Assert.AreEqual(i, TzdbZoneInfoParser.ParseMonth(month));
            }
        }

        [Test]
        public void ParseMonth_longMonthNames()
        {
            for (int i = 1; i < 12; i++)
            {
                var month = new DateTime(2000, i, 1).ToString("MMMM", CultureInfo.InvariantCulture);
                Assert.AreEqual(i, TzdbZoneInfoParser.ParseMonth(month));
            }
        }

        [Test]
        public void ParseZone_badOffset_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("asd US P%sT 1969 Mar 23 14:53:27.856s");
            Assert.Throws(typeof(FormatException), () => parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_emptyString_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_optionalRule()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 - P%sT");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), null, "P%sT", int.MaxValue, ZoneYearOffset.StartOfYear);
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_simple()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", int.MaxValue, ZoneYearOffset.StartOfYear);
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_tooFewWords1_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US");
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_tooFewWords2_exception()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00");
            Assert.Throws(typeof(InvalidDataException), () => parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYear()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, false, LocalTime.Midnight));
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDay()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, 23, 0, false, LocalTime.Midnight));
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTime()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, 23, 0, false, new LocalTime(14, 53, 27, 856)));
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTimeZone()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856s");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Standard, 3, 23, 0, false, new LocalTime(14, 53, 27, 856)));
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void ParseZone_withDayOfWeek()
        {
            var parser = new TzdbZoneInfoParser();
            var tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar lastSun");
            var expected = new ZoneLine(string.Empty, ToOffset(2, 0), "US", "P%sT", 1969, new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int) IsoDayOfWeek.Sunday, false, LocalTime.Midnight));
            Assert.AreEqual(expected, parser.ParseZone(string.Empty, tokens));
        }

        [Test]
        public void Parse_threeLines()
        {
            const string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT\n" +
                "         3:00 -  P%sT\n";
            var database = ParseText(text);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones.Values.Single().Count);
        }

        [Test]
        public void Parse_threeLinesWithComment()
        {
            const string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n";
            var database = ParseText(text);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones.Values.Single().Count);
        }

        [Test]
        public void Parse_twoLines()
        {
            const string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT\n";
            var database = ParseText(text);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones.Values.Single().Count);
        }

        [Test]
        public void Parse_twoLinks()
        {
            const string text =
                "# First line must be a comment\n" +
                "Link from to\n" + "Link target source\n";
            var database = ParseText(text);
            ValidateCounts(database, 0, 0, 2);
        }

        [Test]
        public void Parse_twoZones()
        {
            const string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n" +
                "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" +
                "         3:00 -  E%sT\n";
            var database = ParseText(text);
            ValidateCounts(database, 0, 2, 0);
            Assert.AreEqual(3, database.Zones["PST"].Count);
            Assert.AreEqual(2, database.Zones["EST"].Count);
        }

        [Test]
        public void Parse_twoZonesTwoRule()
        {
            const string text =
                "# A comment\n" +
                "Rule US 1987 2006 - Apr Sun>=1 2:00 1:00 D\n" +
                "Rule US 2007 max  - Mar Sun>=8 2:00 1:00 D\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n" + "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" +
                "         3:00 -  E%sT\n";
            var database = ParseText(text);
            ValidateCounts(database, 1, 2, 0);
            Assert.AreEqual(3, database.Zones["PST"].Count);
            Assert.AreEqual(2, database.Zones["EST"].Count);
        }

        [Test]
        public void Parse_2400_FromDay_AtLeast_Sunday()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Apr Sun>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 4, 2, 0, 0).ToLocalInstant();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay_AtMost_Sunday()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Apr Sun<=7  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 4, 2, 0, 0).ToLocalInstant();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay_AtLeast_Wednesday()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Apr Wed>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 4, 5, 0, 0).ToLocalInstant();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay_AtMost_Wednesday()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Apr Wed<=14  24:00";
            var tokens = Tokens.Tokenize(text);
            var rule = parser.ParseDateTimeOfYear(tokens, true);
            var actual = rule.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 4, 12, 0, 0).ToLocalInstant();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_FromDay()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Apr Sun>=1  24:00";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 4, 1, (int)IsoDayOfWeek.Sunday, true, LocalTime.Midnight, true);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_2400_Last()
        {
            var parser = new TzdbZoneInfoParser();
            const string text = "Mar lastSun 24:00";
            var tokens = Tokens.Tokenize(text);
            var actual = parser.ParseDateTimeOfYear(tokens, true);
            var expected = new ZoneYearOffset(TransitionMode.Wall, 3, -1, (int)IsoDayOfWeek.Sunday, false, LocalTime.Midnight, true);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Parse_Fixed_Eastern()
        {
            const string text =
                "# A comment\n" +
                "Zone\tEtc/GMT-9\t9\t-\tGMT-9\n";
            var database = ParseText(text);

            ValidateCounts(database, 0, 1, 0);
            var zone = database.Zones["Etc/GMT-9"].Single();
            Assert.AreEqual(Offset.FromHours(9), zone.StandardOffset);
            Assert.IsNull(zone.Rules);
            Assert.AreEqual(int.MaxValue, zone.UntilYear);
        }

        [Test]
        public void Parse_Fixed_Western()
        {
            const string text =
                "# A comment\n" +
                "Zone\tEtc/GMT+9\t-9\t-\tGMT+9\n";
            var database = ParseText(text);

            ValidateCounts(database, 0, 1, 0);
            var zone = database.Zones["Etc/GMT+9"].Single();
            Assert.AreEqual(Offset.FromHours(-9), zone.StandardOffset);
            Assert.IsNull(zone.Rules);
            Assert.AreEqual(int.MaxValue, zone.UntilYear);
        }

        /// <summary>
        /// Helper method to create a database and call Parse with the given text.
        /// </summary>
        private TzdbDatabase ParseText(string line)
        {
            var parser = new TzdbZoneInfoParser();
            var database = new TzdbDatabase("version");
            parser.Parse(new StringReader(line), database);
            return database;
        }
    }
}
