// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A mutable builder class for <see cref="Period"/> values. Each property can
    /// be set independently, and then a Period can be created from the result
    /// using the <see cref="Build"/> method.
    /// </summary>
    /// <threadsafety>
    /// This type is not thread-safe without extra synchronization, but has no
    /// thread affinity.
    /// </threadsafety>
    public sealed class PeriodBuilder : IXmlSerializable
    {
        #region Properties
        /// <summary>
        /// Gets or sets the number of years within the period.
        /// </summary>
        public long Years { get; set; }

        /// <summary>
        /// Gets or sets the number of months within the period.
        /// </summary>
        public long Months { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks within the period.
        /// </summary>
        public long Weeks { get; set; }

        /// <summary>
        /// Gets or sets the number of days within the period.
        /// </summary>
        public long Days { get; set; }

        /// <summary>
        /// Gets or sets the number of hours within the period.
        /// </summary>
        public long Hours { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes within the period.
        /// </summary>
        public long Minutes { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds within the period.
        /// </summary>
        public long Seconds { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds within the period.
        /// </summary>
        public long Milliseconds { get; set; }

        /// <summary>
        /// Gets or sets the number of ticks within the period.
        /// </summary>
        public long Ticks { get; set; }
        #endregion

        /// <summary>
        /// Creates a new period builder with an initially zero period.
        /// </summary>
        public PeriodBuilder()
        {
        }

        /// <summary>
        /// Creates a new period builder with the values from an existing
        /// period. Calling this constructor instead of <see cref="Period.ToBuilder"/>
        /// allows object initializers to be used.
        /// </summary>
        /// <param name="period">An existing period to copy values from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="period"/> is null.</exception>
        public PeriodBuilder(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Years = period.Years;
            Months = period.Months;
            Weeks = period.Weeks;
            Days = period.Days;
            Hours = period.Hours;
            Minutes = period.Minutes;
            Seconds = period.Seconds;
            Milliseconds = period.Milliseconds;
            Ticks = period.Ticks;
        }

        /// <summary>
        /// Gets or sets the value of a single unit.
        /// </summary>
        /// <param name="unit">A single value within the <see cref="PeriodUnits"/> enumeration.</param>
        /// <returns>The value of the given unit within this period builder, or zero if the unit is unset.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="unit"/> is not a single unit.</exception>
        public long this[PeriodUnits unit]
        {
            get
            {
                switch (unit)
                {
                    case PeriodUnits.Years: return Years;
                    case PeriodUnits.Months: return Months;
                    case PeriodUnits.Weeks: return Weeks;
                    case PeriodUnits.Days: return Days;
                    case PeriodUnits.Hours: return Hours;
                    case PeriodUnits.Minutes: return Minutes;
                    case PeriodUnits.Seconds: return Seconds;
                    case PeriodUnits.Milliseconds: return Milliseconds;
                    case PeriodUnits.Ticks: return Ticks;
                    default: throw new ArgumentOutOfRangeException("unit", "Indexer for PeriodBuilder only takes a single unit");
                }
            }
            set
            {
                switch (unit)
                {
                    case PeriodUnits.Years: Years = value; return;
                    case PeriodUnits.Months: Months = value; return;
                    case PeriodUnits.Weeks: Weeks = value; return;
                    case PeriodUnits.Days: Days = value; return;
                    case PeriodUnits.Hours: Hours = value; return;
                    case PeriodUnits.Minutes: Minutes = value; return;
                    case PeriodUnits.Seconds: Seconds = value; return;
                    case PeriodUnits.Milliseconds: Milliseconds = value; return;
                    case PeriodUnits.Ticks: Ticks = value; return;
                    default: throw new ArgumentOutOfRangeException("unit", "Indexer for PeriodBuilder only takes a single unit");
                }
            }
        }

        /// <summary>
        /// Builds a period from the properties in this builder.
        /// </summary>
        /// <returns>A period containing the values from this builder.</returns>
        public Period Build()
        {
            return new Period(Years, Months, Weeks, Days, Hours, Minutes, Seconds, Milliseconds, Ticks);
        }
        
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string text = reader.ReadElementContentAsString();
            Period period = PeriodPattern.RoundtripPattern.Parse(text).Value;
            Years = period.Years;
            Months = period.Months;
            Weeks = period.Weeks;
            Days = period.Days;
            Hours = period.Hours;
            Minutes = period.Minutes;
            Seconds = period.Seconds;
            Milliseconds = period.Milliseconds;
            Ticks = period.Ticks;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(PeriodPattern.RoundtripPattern.Format(Build()));
        }
    }
}
