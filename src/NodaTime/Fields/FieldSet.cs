// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// An immutable collection of date/time and period fields.
    /// </summary>
    internal sealed class FieldSet
    {
        private readonly IPeriodField ticks;
        private readonly IPeriodField milliseconds;
        private readonly IPeriodField seconds;
        private readonly IPeriodField minutes;
        private readonly IPeriodField hours;
        private readonly IPeriodField days;
        private readonly IPeriodField weeks;
        private readonly IPeriodField months;
        private readonly IPeriodField years;

        internal IPeriodField Ticks { get { return ticks; } }
        internal IPeriodField Milliseconds { get { return milliseconds; } }
        internal IPeriodField Seconds { get { return seconds; } }
        internal IPeriodField Minutes { get { return minutes; } }
        internal IPeriodField Hours { get { return hours; } }
        internal IPeriodField Days { get { return days; } }
        internal IPeriodField Weeks { get { return weeks; } }
        internal IPeriodField Months { get { return months; } }
        internal IPeriodField Years { get { return years; } }

        private FieldSet(Builder builder)
        {
            ticks = builder.Ticks ?? UnsupportedPeriodField.Ticks;
            milliseconds = builder.Milliseconds ?? UnsupportedPeriodField.Milliseconds;
            seconds = builder.Seconds ?? UnsupportedPeriodField.Seconds;
            minutes = builder.Minutes ?? UnsupportedPeriodField.Minutes;
            hours = builder.Hours ?? UnsupportedPeriodField.Hours;
            days = builder.Days ?? UnsupportedPeriodField.Days;
            weeks = builder.Weeks ?? UnsupportedPeriodField.Weeks;
            months = builder.Months ?? UnsupportedPeriodField.Months;
            years = builder.Years ?? UnsupportedPeriodField.Years;
        }
        
        /// <summary>
        /// Mutable set of fields which can be built into a full, immutable FieldSet.
        /// </summary>
        internal sealed class Builder
        {
            internal IPeriodField Ticks { get; set; }
            internal IPeriodField Milliseconds { get; set; }
            internal IPeriodField Seconds { get; set; }
            internal IPeriodField Minutes { get; set; }
            internal IPeriodField Hours { get; set; }
            internal IPeriodField HalfDays { get; set; }
            internal IPeriodField Days { get; set; }
            internal IPeriodField Weeks { get; set; }
            internal IPeriodField Months { get; set; }
            internal IPeriodField Years { get; set; }

            internal Builder()
            {
            }

            internal Builder(FieldSet baseSet)
            {
                Preconditions.CheckNotNull(baseSet, "baseSet");
                Ticks = baseSet.Ticks;
                Milliseconds = baseSet.Milliseconds;
                Seconds = baseSet.Seconds;
                Minutes = baseSet.Minutes;
                Hours = baseSet.Hours;
                Days = baseSet.Days;
                Weeks = baseSet.Weeks;
                Months = baseSet.Months;
                Years = baseSet.Years;
            }

            internal FieldSet Build()
            {
                return new FieldSet(this);
            }
        }
    }
}