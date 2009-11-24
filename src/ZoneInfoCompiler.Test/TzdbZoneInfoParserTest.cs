#region Copyright and license information
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

using System;
using System.Collections;
using System.Collections.Generic;
using NodaTime.ZoneInfoCompiler;
using NUnit.Framework;
using NodaTime.ZoneInfoCompiler.Tzdb;
using System.IO;

namespace ZoneInfoCompiler.Test
{
    /// <summary>
    /// This is a test class for containing all of the TzdbZoneInfoParser unit tests.
    ///</summary>
    [TestFixture]
    public partial class TzdbZoneInfoParserTest
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

        /* ############################################################################### */

        [Test]
        public void ParseDateTimeOfYear_emptyString()
        {
            string text = string.Empty;
            Tokens tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens));
        }

        [Test]
        public void ParseDateTimeOfYear_missingOn()
        {
            string text = "Mar";
            Tokens tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens));
        }

        [Test]
        public void ParseDateTimeOfYear_missingAt()
        {
            string text = "Mar lastSun";
            Tokens tokens = Tokens.Tokenize(text);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseDateTimeOfYear(tokens));
        }

        [Test]
        public void ParseDateTimeOfYear_onLast()
        {
            string text = "Mar lastTue 2:00";
            Tokens tokens = Tokens.Tokenize(text);
            DateTimeOfYear actual = Parser.ParseDateTimeOfYear(tokens);
            DateTimeOfYear expected = new DateTimeOfYear() {
                MonthOfYear = 3,
                DayOfMonth = -1,
                DayOfWeek = 3,
                MillisecondOfDay = ToMilliseconds(2, 0, 0, 0)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onAfter()
        {
            string text = "Mar Tue>=14 2:00";
            Tokens tokens = Tokens.Tokenize(text);
            DateTimeOfYear actual = Parser.ParseDateTimeOfYear(tokens);
            DateTimeOfYear expected = new DateTimeOfYear() {
                MonthOfYear = 3,
                DayOfMonth = 14,
                DayOfWeek = 3,
                MillisecondOfDay = ToMilliseconds(2, 0, 0, 0),
                AdvanceDayOfWeek = true
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseDateTimeOfYear_onBefore()
        {
            string text = "Mar Tue<=14 2:00";
            Tokens tokens = Tokens.Tokenize(text);
            DateTimeOfYear actual = Parser.ParseDateTimeOfYear(tokens);
            DateTimeOfYear expected = new DateTimeOfYear() {
                MonthOfYear = 3,
                DayOfMonth = 14,
                DayOfWeek = 3,
                MillisecondOfDay = ToMilliseconds(2, 0, 0, 0),
                AdvanceDayOfWeek = false
            };
            Assert.AreEqual(expected, actual);
        }

        /* ############################################################################### */

        [Test]
        public void Parse_emptyStream()
        {
            StringReader reader = new StringReader(string.Empty);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void Parse_twoLines()
        {
            string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_threeLines()
        {
            string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT\n" +
                "         3:00 -  P%sT\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_threeLinesWithComment()
        {
            string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void Parse_twoZones()
        {
            string text =
                "# A comment\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n" +
                "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" +
                "         3:00 -  E%sT\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 2, 0);
            Assert.AreEqual(3, database.Zones[0].Count, "Zones in set " + database.Zones[0].Name);
            Assert.AreEqual(2, database.Zones[1].Count, "Zones in set " + database.Zones[0].Name);
        }

        [Test]
        public void Parse_twoZonesTwoRule()
        {
            string text =
                "# A comment\n" +
                "Rule US 1987 2006 - Apr Sun>=1 2:00 1:00 D\n" +
                "Rule US 2007 max  - Mar Sun>=8 2:00 1:00 D\n" +
                "Zone PST 2:00 US P%sT # An end of line comment\n" +
                "         3:00 -  P%sT\n" +
                "         4:00 -  P%sT\n" +
                "Zone EST 2:00 US E%sT # An end of line comment\n" +
                "         3:00 -  E%sT\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 1, 2, 0);
            Assert.AreEqual(3, database.Zones[0].Count, "Zones in set " + database.Zones[0].Name);
            Assert.AreEqual(2, database.Zones[1].Count, "Zones in set " + database.Zones[0].Name);
        }

        [Test]
        public void Parse_twoLinks()
        {
            string text =
                "Link from to\n" +
                "Link target source\n";
            StringReader reader = new StringReader(text);
            TzdbDatabase database = new TzdbDatabase();
            Parser.Parse(reader, database);
            ValidateCounts(database, 0, 0, 2);
        }

        /* ############################################################################### */

        [Test]
        public void ParseLine_emptyString()
        {
            string line = string.Empty;
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_whiteSpace()
        {
            string line = "    \t\t\n";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_comment()
        {
            string line = "# Comment";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_commentWithLeadingWhitespace()
        {
            string line = "   # Comment";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 0);
        }

        [Test]
        public void ParseLine_zone()
        {
            string line = "Zone PST 2:00 US P%sT";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(1, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void ParseLine_zonePlus()
        {
            string line = "Zone PST 2:00 US P%sT";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);

            line = "  3:00 US P%sT";
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 1, 0);
            Assert.AreEqual(2, database.Zones[0].Count, "Zones in set");
        }

        [Test]
        public void ParseLine_link()
        {
            string line = "Link from to";
            TzdbDatabase database = new TzdbDatabase();
            Parser.ParseLine(line, database);
            ValidateCounts(database, 0, 0, 1);
        }

        /* ############################################################################### */

        [Test]
        public void ParseLink_emptyString_exception()
        {
            Tokens tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseLink(tokens));
        }

        [Test]
        public void ParseLink_tooFewWords_exception()
        {
            Tokens tokens = Tokens.Tokenize("from");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseLink(tokens));
        }

        [Test]
        public void ParseLink_simple()
        {
            Tokens tokens = Tokens.Tokenize("from to");
            ZoneAlias actual = Parser.ParseLink(tokens);
            ZoneAlias expected = new ZoneAlias("from", "to");
            Assert.AreEqual(expected, actual);
        }

        /* ############################################################################### */

        [Test]
        public void ParseZone_emptyString_exception()
        {
            Tokens tokens = Tokens.Tokenize(string.Empty);
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_tooFewWords2_exception()
        {
            Tokens tokens = Tokens.Tokenize("2:00");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_tooFewWords1_exception()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US");
            Assert.Throws(typeof(TzdbZoneInfoParser.ParseException), () => Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_simple()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US P%sT");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT"
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_optionalRule()
        {
            Tokens tokens = Tokens.Tokenize("2:00 - P%sT");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = null,
                Format = "P%sT"
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_withYear()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US P%sT 1969");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT",
                Year = 1969
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDay()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT",
                Year = 1969,
                Month = 3,
                Day = 23
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTime()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT",
                Year = 1969,
                Month = 3,
                Day = 23,
                Millisecond = ToMilliseconds(14, 53, 27, 856)
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_withYearMonthDayTimeZone()
        {
            Tokens tokens = Tokens.Tokenize("2:00 US P%sT 1969 Mar 23 14:53:27.856s");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT",
                Year = 1969,
                Month = 3,
                Day = 23,
                Millisecond = ToMilliseconds(14, 53, 27, 856),
                ZoneCharacter = 's'
            };
            Assert.AreEqual(expected, Parser.ParseZone(tokens));
        }

        [Test]
        public void ParseZone_badOffset_exception()
        {
            Tokens tokens = Tokens.Tokenize("asd US P%sT 1969 Mar 23 14:53:27.856s");
            Zone expected = new Zone() {
                OffsetMilliseconds = ToMilliseconds(2, 0, 0, 0),
                Rules = "US",
                Format = "P%sT",
                Year = 1969,
                Month = 3,
                Day = 23,
                Millisecond = ToMilliseconds(14, 53, 27, 856),
                ZoneCharacter = 's'
            };
            Assert.Throws(typeof(FormatException), () => Parser.ParseZone(tokens));
        }

        private static int ToMilliseconds(int hours, int minutes, int seconds, int fractions)
        {
            return (((((hours * 60) + minutes) * 60) + seconds) * 1000) + fractions;
        }

        /* ############################################################################### */

        [Test]
        public void ParseMonth_nullArgument_default()
        {
            string month = null;
            Assert.AreEqual(0, Parser.ParseMonth(month));
        }

        [Test]
        public void ParseMonth_emptyString_default()
        {
            string month = string.Empty;
            Assert.AreEqual(0, Parser.ParseMonth(month));
        }

        [Test]
        public void ParseMonth_invalidMonth_default()
        {
            string month = "Able";
            Assert.AreEqual(0, Parser.ParseMonth(month));
        }

        [Test]
        public void ParseMonth_months()
        {
            for (int i = 0; i < MonthNames.Length; i++) {
                string month = MonthNames[i];
                Assert.AreEqual(i + 1, Parser.ParseMonth(month));
            }
        }

        /* ############################################################################### */

        private void ValidateCounts(TzdbDatabase database, int ruleSets, int zoneLists, int links)
        {
            Assert.AreEqual(ruleSets, database.Rules.Count, "Rules");
            Assert.AreEqual(zoneLists, database.Zones.Count, "Zones");
            Assert.AreEqual(links, database.Aliases.Count, "Links");
        }
    }
}
