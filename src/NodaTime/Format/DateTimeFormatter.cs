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

using System;
using System.Text;
using System.IO;

namespace NodaTime.Format
{
    /// <summary>
    /// Original name: DateTimeFormatter.
    /// Possible rename: DateTimePattern, given that this is used for both parsing
    /// and formatting...
    /// Or we could rename it differently based on what we rename DateTime to in the end...
    /// </summary>
    public class DateTimeFormatter
    {
        // The internal printer used to output the datetime.
        private readonly IDateTimePrinter printer;
        // The internal parser used to output the datetime.
        private readonly IDateTimeParser parser;
        // The locale to use for printing and parsing.
        private readonly IFormatProvider locale;
        // Whether the offset is parsed.
        private readonly bool offsetParsed;
        // The chronology to use as an override.
        private readonly Chronology chronology;
        // The zone to use as an override.
        private readonly IDateTimeZone zone;
        // The pivot year to use for two-digit year parsing.
        private readonly int? pivotYear;

        public DateTimeFormatter(
              IDateTimePrinter printer, IDateTimeParser parser)
        {
            this.printer = printer;
            this.parser = parser;
            locale = null;
            offsetParsed = false;
            chronology = null;
            zone = null;
            pivotYear = null;
        }

        private DateTimeFormatter(
            IDateTimePrinter printer, IDateTimeParser parser,
            IFormatProvider locale, bool offsetParsed,
            Chronology chronology, IDateTimeZone zone,
            int? pivotYear)
        {
            this.printer = printer;
            this.parser = parser;
            this.locale = locale;
            this.offsetParsed = offsetParsed;
            this.chronology = chronology;
            this.zone = zone;
            this.pivotYear = pivotYear;
        }

        #region Properties and With* methods

        public bool IsPrinter { get { return printer != null; } }
        public IDateTimePrinter Printer { get { return printer; } }

        public bool IsParser { get { return parser != null; } }
        public IDateTimeParser Parser { get { return parser; } }

        public IFormatProvider Locale { get { return locale; } }
        public DateTimeFormatter WithLocale(IFormatProvider newLocale)
        {
            if (this.locale == newLocale)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, newLocale, offsetParsed, chronology, zone, pivotYear);
        }

        public bool IsOffsetParsed { get { return offsetParsed; } }
        public DateTimeFormatter WithOffsetParsed()
        {
            const bool newOffsetParsed = true; // this const is for consistency with all other With* methods
            if (this.offsetParsed == newOffsetParsed)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, locale, newOffsetParsed, chronology, zone, pivotYear);
        }

        public Chronology Chronology { get { return chronology; } }
        public DateTimeFormatter WithChronology(Chronology newChronology)
        {
            if (this.chronology == newChronology)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, locale, offsetParsed, newChronology, zone, pivotYear);
        }

        public IDateTimeZone Zone { get { return zone; } }
        public DateTimeFormatter WithZone(IDateTimeZone newZone)
        {
            if (this.zone == newZone)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, locale, offsetParsed, chronology, newZone, pivotYear);
        }

        public int? PivotYear { get { return pivotYear; } }
        public DateTimeFormatter WithPivotYear(int? newPivotYear)
        {
            if (this.pivotYear == newPivotYear)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, locale, offsetParsed, chronology, zone, newPivotYear);
        }

        #endregion

        #region Printing

        public void PrintTo(StringBuilder builder, Instant instant)
        {
            Chronology chronology;
            Offset timezoneOffset;
            LocalInstant adjustedInstant;
            PrepareToPrint(instant, out chronology, out timezoneOffset, out adjustedInstant);

            printer.PrintTo(builder, adjustedInstant, chronology.CalendarSystem, timezoneOffset, chronology.Zone, locale);
        }

        public void PrintTo(Stream stream, Instant instant)
        {
            Chronology chronology;
            Offset timezoneOffset;
            LocalInstant adjustedInstant;
            PrepareToPrint(instant, out chronology, out timezoneOffset, out adjustedInstant);

            printer.PrintTo(stream, adjustedInstant, chronology.CalendarSystem, timezoneOffset, chronology.Zone, locale);
        }

        public void PrintTo(StringBuilder builder, IPartial partial)
        {
            if (partial == null)
                throw new ArgumentNullException("partial");
            RequirePrinter();

            printer.PrintTo(builder, partial, locale);
        }

        public void PrintTo(Stream stream, IPartial partial)
        {
            if (partial == null)
                throw new ArgumentNullException("partial");
            RequirePrinter();

            printer.PrintTo(stream, partial, locale);
        }

        public string Print(Instant instant)
        {
            RequirePrinter();

            StringBuilder builder = new StringBuilder(printer.EstimatedPrintedLength);
            PrintTo(builder, instant);
            return builder.ToString();
        }

        public string Print(IPartial partial)
        {
            RequirePrinter();

            StringBuilder builder = new StringBuilder(printer.EstimatedPrintedLength);
            PrintTo(builder, partial);
            return builder.ToString();
        }

        private void PrepareToPrint(Instant instant, out Chronology chronology, out Offset timezoneOffset, out LocalInstant adjustedInstant)
        {
            RequirePrinter();

            chronology = SelectChronology(instant, this.chronology);
            timezoneOffset = chronology.Zone.GetOffsetFromUtc(instant);
            adjustedInstant = instant + timezoneOffset;
        }

        #endregion

        #region RequirePrinter, RequireParser

        private void RequirePrinter()
        {
            if (printer == null)
                throw new InvalidOperationException("Printing is not supported");
        }

        private void RequireParser()
        {
            if (parser == null)
                throw new InvalidOperationException("Parser is not supported");
        }

        #endregion

        private static Chronology SelectChronology(Instant instant, Chronology thisChronology)
        {
            // TODO: Java has the line
            // Chronology chrono = DateTimeUtils.getInstantChronology(instant);
            // I'm not sure what I'm supposed to do with that...

            Chronology instantChronology = null; // TODO
            return thisChronology ?? instantChronology ?? Chronology.IsoUtc;
            // return first one which isn't null
        }
    }
}
