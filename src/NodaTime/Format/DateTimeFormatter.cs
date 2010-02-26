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
using System.IO;
using System.Text;
using NodaTime.Calendars;

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
        private readonly IFormatProvider provider;
        // Whether the offset is parsed.
        private readonly bool offsetParsed;
        // The calendar system to use as an override.
        private readonly ICalendarSystem calendarSystem;
        // The zone to use as an override.
        private readonly IDateTimeZone zone;
        // The pivot year to use for two-digit year parsing.
        private readonly int? pivotYear;

        public DateTimeFormatter(
              IDateTimePrinter printer, IDateTimeParser parser)
        {
            this.printer = printer;
            this.parser = parser;
            provider = null;
            offsetParsed = false;
            calendarSystem = null;
            zone = null;
            pivotYear = null;
        }

        private DateTimeFormatter(
            IDateTimePrinter printer, IDateTimeParser parser,
            IFormatProvider locale, bool offsetParsed,
            ICalendarSystem calendarSystem, IDateTimeZone zone,
            int? pivotYear)
        {
            this.printer = printer;
            this.parser = parser;
            this.provider = locale;
            this.offsetParsed = offsetParsed;
            this.calendarSystem = calendarSystem;
            this.zone = zone;
            this.pivotYear = pivotYear;
        }

        #region Properties and With* methods

        /// <summary>
        /// Indicates whether this formatter capable of printing.
        /// </summary>
        public bool IsPrinter { get { return printer != null; } }

        /// <summary>
        /// Gets the internal printer object that performs the real printing work.
        /// </summary>
        public IDateTimePrinter Printer { get { return printer; } }

        /// <summary>
        /// Indicates whether this formatter capable of printing.
        /// </summary>
        public bool IsParser { get { return parser != null; } }

        /// <summary>
        /// Gets the internal parser object that performs the real parsing work.
        /// </summary>
        public IDateTimeParser Parser { get { return parser; } }

        /// <summary>
        /// Gets the format provider that will be used for printing and parsing.
        /// </summary>
        public IFormatProvider Provider { get { return provider; } }

        /// <summary>
        /// Returns a new formatter with a different format provider that will be used
        /// for printing and parsing.
        /// </summary>
        /// <param name="newProvider">The format provider to use; if null, formatter uses default locale</param>
        /// <returns>The new formatter</returns>
        /// <remarks>A DateTimeFormatter is immutable, so a new instance is returned,
        /// and the original is unaltered and still usable.</remarks>
        public DateTimeFormatter WithProvider(IFormatProvider newProvider)
        {
            if (Object.Equals(this.provider, newProvider))
            {
                // no change, so there's no need to create a new formatter
                return this;
            }

            return new DateTimeFormatter(printer, parser, newProvider, offsetParsed, calendarSystem, zone, pivotYear);
        }

        /// <summary>
        /// Indicates whether the offset from the string is used as the zone of
        /// the parsed datetime.
        /// </summary>
        public bool IsOffsetParsed { get { return offsetParsed; } }

        /// <summary>
        /// Returns a new formatter that will create a datetime with a time zone
        /// equal to that of the offset of the parsed string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// After calling this method, a string '2004-06-09T10:20:30-08:00' will
        /// create a datetime with a zone of -08:00 (a fixed zone, with no daylight
        /// savings rules). If the parsed string represents a local time (no zone
        /// offset) the parsed datetime will be in the default zone.
        /// <para>
        /// Calling this method sets the override zone to null.
        /// Calling the override zone method sets this flag off.
        /// </para>
        /// </remarks>
        public DateTimeFormatter WithOffsetParsed()
        {
            if (offsetParsed)
            {
                // no change, so there's no need to create a new formatter
                return this;
            }

            return new DateTimeFormatter(printer, parser, provider, true, calendarSystem, zone, pivotYear);
        }

        /// <summary>
        /// Gets the calendar system to use as an override.
        /// </summary>
        public ICalendarSystem CalendarSystem { get { return calendarSystem; } }

        /// <summary>
        /// Returns a new formatter that will use the specified calendar system in
        /// preference to that of the printed object, or ISO on a parse.
        /// </summary>
        /// <param name="newCalendarSystem">The calendar system to use as an override</param>
        /// <returns>The new formatter</returns>
        /// <remarks>
        /// <para>
        /// When printing, this calendar system will be used in preference to the calendar system
        /// from the datetime that would otherwise be used.
        /// </para>
        /// <para>
        /// When parsing, this calendar system will be set on the parsed datetime.
        /// </para>
        /// <para>
        /// A null calendar system means no-override.
        /// </para>
        /// </remarks>
        public DateTimeFormatter WithCalendarSystem(ICalendarSystem newCalendarSystem)
        {
            if (Object.Equals(calendarSystem, newCalendarSystem))
            {
                // no change, so there's no need to create a new formatter
                return this;
            }

            return new DateTimeFormatter(printer, parser, provider, offsetParsed, newCalendarSystem, zone, pivotYear);
        }

        /// <summary>
        /// Gets the zone to use as an override.
        /// </summary>
        public IDateTimeZone Zone { get { return zone; } }

        /// <summary>
        /// Returns a new formatter that will use the specified zone in preference
        /// to the zone of the printed object, or default zone on a parse.
        /// </summary>
        /// <param name="newZone">The zone to use as an override</param>
        /// <returns>The new formatter</returns>
        /// <remarks>
        /// <para>
        /// When printing, this zone will be used in preference to the zone
        /// from the datetime that would otherwise be used.
        /// </para>
        /// <para>
        /// When parsing, this zone will be set on the parsed datetime.
        /// </para>
        /// <para>
        /// A null zone means of no-override.
        /// </para>
        /// <para>
        /// </remarks>
        public DateTimeFormatter WithZone(IDateTimeZone newZone)
        {
            if (this.zone == newZone)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, provider, offsetParsed, calendarSystem, newZone, pivotYear);
        }

        /// <summary>
        /// Gets the pivot year to use as an override.
        /// </summary>
        public int? PivotYear { get { return pivotYear; } }

        /// <summary>
        /// Returns a new formatter that will use the specified pivot year for two
        /// digit year parsing in preference to that stored in the parser.
        /// </summary>
        /// <param name="newPivotYear"></param>
        /// <returns></returns>
        public DateTimeFormatter WithPivotYear(int? newPivotYear)
        {
            if (this.pivotYear == newPivotYear)
                // no change, so there's no need to create a new formatter
                return this;

            return new DateTimeFormatter(printer, parser, provider, offsetParsed, calendarSystem, zone, newPivotYear);
        }

        #endregion

        #region Printing

        /// <summary>
        /// Prints a ZonedDateTime to a String.
        /// </summary>
        /// <param name="dateTime">The dateTime to format</param>
        /// <returns></returns>
        /// <remarks>
        /// This method will use the override zone and the override calendar system if
        /// they are set. Otherwise it will use the chronology and zone of the dateTime.
        /// </remarks>
        public string Print(ZonedDateTime dateTime)
        {
            VerifyPrinter();

            var builder = new StringBuilder(printer.EstimatedPrintedLength);
            PrintTo(builder, dateTime);

            return builder.ToString();
        }

        /// <summary>
        /// Prints a ZonedDateTime to the specified string builder.
        /// </summary>
        /// <param name="builder">The builder to use</param>
        /// <param name="dateTime">The dateTime to format</param>
        public void PrintTo(StringBuilder builder, ZonedDateTime dateTime)
        {
            PrintTo(new StringWriter(builder), dateTime);
        }

        public void PrintTo(TextWriter writer, ZonedDateTime dateTime)
        {
            VerifyPrinter();

            ICalendarSystem calendarSystem = SelectCalendarSystem(dateTime);
            IDateTimeZone zone = SelectZone(dateTime);

            var instant = dateTime.ToInstant();
            var timezoneOffset = zone.GetOffsetFromUtc(instant);
            var adjustedLocalInstant = instant + timezoneOffset;

            printer.PrintTo(writer, adjustedLocalInstant, calendarSystem, timezoneOffset, zone, provider);
        }

        public void PrintTo(StringBuilder builder, IPartial partial)
        {
            if (partial == null)
                throw new ArgumentNullException("partial");
            VerifyPrinter();

            printer.PrintTo(new StringWriter(builder), partial, provider);
        }

        #endregion

        private void VerifyPrinter()
        {
            if (printer == null)
                throw new NotSupportedException("Printing is not supported");
        }

        private void VeridyParser()
        {
            if (parser == null)
                throw new NotSupportedException("Parser is not supported");
        }

        private ICalendarSystem SelectCalendarSystem(ZonedDateTime dateTime)
        {
            return calendarSystem == null ? dateTime.Chronology.Calendar : calendarSystem;
        }

        private IDateTimeZone SelectZone(ZonedDateTime dateTime)
        {
            return zone == null ? dateTime.Zone : zone;
        }
    }
}
