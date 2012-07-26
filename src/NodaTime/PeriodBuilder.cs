#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A mutable builder class for <see cref="Period"/> values. Each property can
    /// be set independently, and then a Period can be created from the result
    /// using the <see cref="Build"/> method.
    /// The properties are all nullable: the units of the period created are
    /// determined by the non-null properties at build time.
    /// </summary>
    /// <remarks>
    /// This type is not thread-safe without extra synchronization, but has no
    /// thread affinity. Note that although this method implements
    /// <see cref="IEquatable{T}"/> and overrides <see cref="GetHashCode"/>,
    /// it should generally not be used as a key in a dictionary, as it is mutable. If you
    /// mutate an instance after using it as a key, you may not be able to look it up
    /// again, even using the same reference.
    /// </remarks>
    /// <threadsafety>
    /// This type is not thread-safe, but the periods it builds are.
    /// </threadsafety>
    public sealed class PeriodBuilder : IEquatable<PeriodBuilder>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the number of years within the period. Null means that
        /// the "years" unit is absent.
        /// </summary>
        public long? Years { get; set; }

        /// <summary>
        /// Gets or sets the number of months within the period. Null means that
        /// the "months" unit is absent.
        /// </summary>
        public long? Months { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks within the period. Null means that
        /// the "weeks" unit is absent.
        /// </summary>
        public long? Weeks { get; set; }

        /// <summary>
        /// Gets or sets the number of days within the period. Null means that
        /// the "days" unit is absent.
        /// </summary>
        public long? Days { get; set; }

        /// <summary>
        /// Gets or sets the number of hours within the period. Null means that
        /// the "hours" unit is absent.
        /// </summary>
        public long? Hours { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes within the period. Null means that
        /// the "minutes" unit is absent.
        /// </summary>
        public long? Minutes { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds within the period. Null means that
        /// the "seconds" unit is absent.
        /// </summary>
        public long? Seconds { get; set; }
        
        /// <summary>
        /// Gets or sets the number of milliseconds within the period. Null means that
        /// the "milliseconds" unit is absent.
        /// </summary>
        public long? Milliseconds { get; set; }

        /// <summary>
        /// Gets or sets the number of ticks within the period. Null means that
        /// the "ticks" unit is absent.
        /// </summary>
        public long? Ticks { get; set; }
        #endregion

        /// <summary>
        /// Creates a new period builder with an initially empty period.
        /// </summary>
        public PeriodBuilder()
        {
        }

        /// <summary>
        /// Creates a new period builder with the values (and units) from an existing
        /// period. Calling this constructor instead of <see cref="Period.ToBuilder"/>
        /// allows object initializers to be used.
        /// </summary>
        /// <param name="period">An existing period to copy values from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="period"/> is null</exception>
        public PeriodBuilder(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            var units = period.Units;
            Years = (units & PeriodUnits.Years) == 0 ? (long?)null : period.Years;
            Months = (units & PeriodUnits.Months) == 0 ? (long?)null : period.Months;
            Weeks = (units & PeriodUnits.Weeks) == 0 ? (long?)null : period.Weeks;
            Days = (units & PeriodUnits.Days) == 0 ? (long?)null : period.Days;
            Hours = (units & PeriodUnits.Hours) == 0 ? (long?)null : period.Hours;
            Minutes = (units & PeriodUnits.Minutes) == 0 ? (long?)null : period.Minutes;
            Seconds = (units & PeriodUnits.Seconds) == 0 ? (long?)null : period.Seconds;
            Milliseconds = (units & PeriodUnits.Milliseconds) == 0 ? (long?)null : period.Milliseconds;
            Ticks = (units & PeriodUnits.Milliseconds) == 0 ? (long?)null : period.Ticks;
        }

        /// <summary>
        /// Gets or sets the value of a single unit
        /// </summary>
        /// <param name="unit">A single value within the <see cref="PeriodUnits"/> enumeration.</param>
        /// <returns>The value of the given unit within this period builder, or null if the unit is unset.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="unit"/> is not a single unit</exception>
        public long? this[PeriodUnits unit]
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
        /// <remarks>
        /// Any non-null property contributes the corresponding unit to the returned period (even if the value is 0).
        /// </remarks>
        /// <returns>A period containing the values from this builder.</returns>
        public Period Build()
        {
            PeriodUnits units = 
                (Years == null ? 0 : PeriodUnits.Years) |
                (Months == null ? 0 : PeriodUnits.Months) |
                (Weeks == null ? 0 : PeriodUnits.Weeks) |
                (Days == null ? 0 : PeriodUnits.Days) |
                (Hours == null ? 0 : PeriodUnits.Hours) |
                (Minutes == null ? 0 : PeriodUnits.Minutes) |
                (Seconds == null ? 0 : PeriodUnits.Seconds) |
                (Milliseconds == null ? 0 : PeriodUnits.Milliseconds) |
                (Ticks == null ? 0 : PeriodUnits.Ticks);
            switch (units)
            {
                case PeriodUnits.None: return Period.Empty;
                case PeriodUnits.Years: return Period.FromYears(Years.Value);
                case PeriodUnits.Months: return Period.FromMonths(Months.Value);
                case PeriodUnits.Weeks: return Period.FromWeeks(Weeks.Value);
                case PeriodUnits.Days: return Period.FromDays(Days.Value);
                case PeriodUnits.Hours: return Period.FromHours(Hours.Value);
                case PeriodUnits.Minutes: return Period.FromMinutes(Minutes.Value);
                case PeriodUnits.Seconds: return Period.FromSeconds(Seconds.Value);
                case PeriodUnits.Milliseconds: return Period.FromMillseconds(Milliseconds.Value);
                case PeriodUnits.Ticks: return Period.FromTicks(Ticks.Value);
                default: return Period.UnsafeCreate(units, new[] {
                    Years ?? 0L, Months ?? 0L, Weeks ?? 0L, Days ?? 0L,
                    Hours ?? 0L, Minutes ?? 0L, Seconds ?? 0L, Milliseconds ?? 0L, Ticks ?? 0L
                });
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as PeriodBuilder);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Years);
            hash = HashCodeHelper.Hash(hash, Months);
            hash = HashCodeHelper.Hash(hash, Weeks);
            hash = HashCodeHelper.Hash(hash, Days);
            hash = HashCodeHelper.Hash(hash, Hours);
            hash = HashCodeHelper.Hash(hash, Minutes);
            hash = HashCodeHelper.Hash(hash, Seconds);
            hash = HashCodeHelper.Hash(hash, Milliseconds);
            hash = HashCodeHelper.Hash(hash, Ticks);
            return hash;
        }

        /// <summary>
        /// Indicates whether the value of this period builder is equal to the value of the specified one.
        /// All properties are taken into account without normalization, and units are also considered -
        /// so a builder with a null Hours property is not equal to one with a 0 Hours property,
        /// for example.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this period builder is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(PeriodBuilder other)
        {
            return other != null &&
                this.Years == other.Years &&
                this.Months == other.Months &&
                this.Weeks == other.Weeks &&
                this.Days == other.Days &&
                this.Hours == other.Hours &&
                this.Minutes == other.Minutes &&
                this.Seconds == other.Seconds &&
                this.Milliseconds == other.Milliseconds &&
                this.Ticks == other.Ticks;
        }
    }
}
