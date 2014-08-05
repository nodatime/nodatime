// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// An era for a specific calendar, in order to have specific behaviour
    /// around min/max dates, century and negation behaviour etc. The base implementation is suitable
    /// for calendars where one era follows another, simply resetting the year to 1 at the start of
    /// each new era. Whereas two significantly different calendars may well share the same <see cref="Era"/>, they're unlikely to
    /// share the same <see cref="CalendarEra"/> values, due to differnet limits. (For example, the Gregorian and Julian calendars
    /// both use BCE and CE, but have different ranges.)
    /// </summary>
    internal class CalendarEra
    {
        private readonly Era era;
        /// <summary>
        /// <see cref="Era"/> to which this object applies. Different calendars may
        /// have different <see cref="CalendarEra"/> instances for the same <c>Era</c>
        /// </summary>
        public Era Era { get { return era; } }

        private readonly YearMonthDay minDate;
        /// <summary>
        /// The earliest date (expressed with a <see cref="YearMonthDay"/>, so
        /// in absolute years) within this era (and handled by Noda Time).
        /// </summary>
        public YearMonthDay MinDate { get { return minDate; } }
        private readonly YearMonthDay maxDate;
        /// <summary>
        /// The latest date (expressed with a <see cref="YearMonthDay"/>, so
        /// in absolute years) within this era  (and handled by Noda Time).
        /// </summary>
        public YearMonthDay MaxDate { get { return maxDate; } }

        private readonly int minYearOfEra;
        /// <summary>
        /// The smallest "year of era" within this era.
        /// </summary>
        public int MinYearOfEra { get { return minYearOfEra; } }

        private readonly int maxYearOfEra;
        /// <summary>
        /// The largest "year of era" within this era.
        /// </summary>
        public int MaxYearOfEra { get { return maxYearOfEra; } }

        // False for ISO-8601 handling of centuries (e.g. 101-200 is a century), true for 100-199 handling..
        // This affects both century-of-era and year-of-century.
        private readonly bool isoCenturies;

        internal CalendarEra(Era era, YearMonthDay minDate, YearMonthDay maxDate, bool isoCenturies = false)
        {
            this.era = era;
            this.minDate = minDate;
            this.maxDate = maxDate;
            this.isoCenturies = isoCenturies;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            // Deliberately calling a virtual method within a constructor. We know all the
            // implementations, and we've documented the requirements.
            int minDateYear = GetYearOfEra(minDate);
            int maxDateYear = GetYearOfEra(maxDate);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            minYearOfEra = Math.Min(minDateYear, maxDateYear);
            maxYearOfEra = Math.Max(minDateYear, maxDateYear);
        }

        /// <summary>
        /// Returns the absolute year based on a year-of-era value. This performs no validation.
        /// </summary>
        internal virtual int GetAbsoluteYear(int yearOfEra)
        {
            Preconditions.CheckArgumentRange("yearOfEra", yearOfEra, minYearOfEra, maxYearOfEra);
            return yearOfEra + minDate.Year - 1;
        }

        /// <summary>
        /// Returns the year of era based on an absolute year/month/day. The default
        /// implementation just returns the year. Implementations may rely on the
        /// Era, MinDate and MaxDate being set, but *not* MinYearOfEra or MaxYearOfEra.
        /// </summary>
        internal virtual int GetYearOfEra(YearMonthDay yearMonthDay)
        {
            // Note: this could be optimized for single-calendar eras, but it's not worth the effort.
            return yearMonthDay.Year - minDate.Year + 1;
        }

        /// <summary>
        /// Returns the year of the century (of the era).
        /// </summary>
        internal virtual int GetYearOfCentury(YearMonthDay yearMonthDay)
        {
            int yearOfEra = GetYearOfEra(yearMonthDay);
            int zeroBased = yearOfEra % 100;
            return zeroBased == 0 && !isoCenturies ? 100 : zeroBased;
        }

        /// <summary>
        /// Returns the century number within the era.
        /// </summary>
        internal virtual int GetCenturyOfEra(YearMonthDay yearMonthDay)
        {
            int yearOfEra = GetYearOfEra(yearMonthDay);
            int zeroBasedRemainder = yearOfEra % 100;
            int zeroBasedResult = yearOfEra / 100;
            return zeroBasedRemainder == 0 || isoCenturies ? zeroBasedResult : zeroBasedResult + 1;
        }

        // Subclass of CalendarEra for the "before common" era, where years increase the further back in time you go.
        // It is assumed that 0 absolute = 1BC, 1 absolute = 2BC etc.
        // Century handling can be performed in an ISO-compatible way or a "normal" way - it's not at all clear
        // that the ISO handling is even slightly desirable at the moment.
        internal class BcCalendarEra : CalendarEra
        {
            internal BcCalendarEra(YearMonthDay minDate, bool isoCenturies)
                : base(Era.BeforeCommon, minDate, new YearMonthDay(0, 12, 31), isoCenturies)
            {
            }

            internal override int GetYearOfEra(YearMonthDay yearMonthDay)
            {
                return 1 - yearMonthDay.Year;
            }

            // Okay, this is nasty... but hopefully it's only temporary.
            internal override int GetCenturyOfEra(YearMonthDay yearMonthDay)
            {
                return isoCenturies ? Math.Abs(yearMonthDay.Year) / 100 : base.GetCenturyOfEra(yearMonthDay);
            }

            internal override int GetYearOfCentury(YearMonthDay yearMonthDay)
            {
                return isoCenturies ? Math.Abs(yearMonthDay.Year) % 100 : base.GetYearOfCentury(yearMonthDay);
            }

            internal override int GetAbsoluteYear(int yearOfEra)
            {
                Preconditions.CheckArgumentRange("yearOfEra", yearOfEra, minYearOfEra, maxYearOfEra);
                return 1 - yearOfEra;
            }
        }

    }
}
