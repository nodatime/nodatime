// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
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
    [Mutable]
    public sealed class PeriodBuilder : IXmlSerializable
    {
        #region Properties
        /// <summary>
        /// Gets or sets the number of years within the period.
        /// </summary>
        /// <value>The number of years within the period.</value>
        public int Years { get; set; }

        /// <summary>
        /// Gets or sets the number of months within the period.
        /// </summary>
        /// <value>The number of months within the period.</value>
        public int Months { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks within the period.
        /// </summary>
        /// <value>The number of weeks within the period.</value>
        public int Weeks { get; set; }

        /// <summary>
        /// Gets or sets the number of days within the period.
        /// </summary>
        /// <value>The number of days within the period.</value>
        public int Days { get; set; }

        /// <summary>
        /// Gets or sets the number of hours within the period.
        /// </summary>
        /// <value>The number of hours within the period.</value>
        public long Hours { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes within the period.
        /// </summary>
        /// <value>The number of minutes within the period.</value>
        public long Minutes { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds within the period.
        /// </summary>
        /// <value>The number of seconds within the period.</value>
        public long Seconds { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds within the period.
        /// </summary>
        /// <value>The number of milliseconds within the period.</value>
        public long Milliseconds { get; set; }

        /// <summary>
        /// Gets or sets the number of ticks within the period.
        /// </summary>
        /// <value>The number of ticks within the period.</value>
        public long Ticks { get; set; }

        /// <summary>
        /// Gets or sets the number of nanoseconds within the period.
        /// </summary>
        /// <value>The number of nanoseconds within the period.</value>
        public long Nanoseconds { get; set; }
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
        public PeriodBuilder([NotNull] Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            Years = period.Years;
            Months = period.Months;
            Weeks = period.Weeks;
            Days = period.Days;
            Hours = period.Hours;
            Minutes = period.Minutes;
            Seconds = period.Seconds;
            Milliseconds = period.Milliseconds;
            Ticks = period.Ticks;
            Nanoseconds = period.Nanoseconds;
        }

        /// <summary>
        /// Gets or sets the value of a single unit.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The type of this indexer is <see cref="System.Int64"/> for uniformity, but any date unit (year, month, week, day) will only ever have a value
        /// in the range of <see cref="System.Int32"/>.
        /// </para>
        /// <para>
        /// For the <see cref="PeriodUnits.Nanoseconds"/> unit, the value is converted to <c>Int64</c> when reading from the indexer, causing it to
        /// fail if the value is out of range (around 250 years). To access the values of very large numbers of nanoseconds, use the <see cref="Nanoseconds"/>
        /// property directly.
        /// </para>
        /// </remarks>
        /// <param name="unit">A single value within the <see cref="PeriodUnits"/> enumeration.</param>
        /// <value>The value of the given unit within this period builder, or zero if the unit is unset.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="unit"/> is not a single unit, or a value is provided for a date unit which is outside the range of <see cref="System.Int32"/>.</exception>
        public long this[PeriodUnits unit]
        {
            get => unit switch
                {
                    PeriodUnits.Years => Years,
                    PeriodUnits.Months => Months,
                    PeriodUnits.Weeks => Weeks,
                    PeriodUnits.Days => Days,
                    PeriodUnits.Hours => Hours,
                    PeriodUnits.Minutes => Minutes,
                    PeriodUnits.Seconds => Seconds,
                    PeriodUnits.Milliseconds => Milliseconds,
                    PeriodUnits.Ticks => Ticks,
                    PeriodUnits.Nanoseconds => Nanoseconds,
                    _ => throw new ArgumentOutOfRangeException(nameof(unit), "Indexer for PeriodBuilder only takes a single unit")
                };
            set
            {
                if ((unit & PeriodUnits.AllDateUnits) != 0)
                {
                    Preconditions.CheckArgumentRange(nameof(value), value, int.MinValue, int.MaxValue);
                }
                switch (unit)
                {
                    case PeriodUnits.Years: Years = (int) value; return;
                    case PeriodUnits.Months: Months = (int) value; return;
                    case PeriodUnits.Weeks: Weeks = (int) value; return;
                    case PeriodUnits.Days: Days = (int) value; return;
                    case PeriodUnits.Hours: Hours = value; return;
                    case PeriodUnits.Minutes: Minutes = value; return;
                    case PeriodUnits.Seconds: Seconds = value; return;
                    case PeriodUnits.Milliseconds: Milliseconds = value; return;
                    case PeriodUnits.Ticks: Ticks = value; return;
                    case PeriodUnits.Nanoseconds: Nanoseconds = value; return;
                    default: throw new ArgumentOutOfRangeException(nameof(unit), "Indexer for PeriodBuilder only takes a single unit");
                }
            }
        }

        /// <summary>
        /// Builds a period from the properties in this builder.
        /// </summary>
        /// <returns>A period containing the values from this builder.</returns>
        [NotNull] public Period Build() =>
            new Period(Years, Months, Weeks, Days, Hours, Minutes, Seconds, Milliseconds, Ticks, Nanoseconds);

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string text = reader.ReadElementContentAsString();
            Period period = PeriodPattern.Roundtrip.Parse(text).Value;
            Years = period.Years;
            Months = period.Months;
            Weeks = period.Weeks;
            Days = period.Days;
            Hours = period.Hours;
            Minutes = period.Minutes;
            Seconds = period.Seconds;
            Milliseconds = period.Milliseconds;
            Ticks = period.Ticks;
            Nanoseconds = period.Nanoseconds;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(PeriodPattern.Roundtrip.Format(Build()));
        }
    }
}
