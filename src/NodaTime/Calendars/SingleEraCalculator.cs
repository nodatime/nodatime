// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of <see cref="EraCalculator"/> for calendars which only have a single era.
    /// </summary>
    internal sealed class SingleEraCalculator : EraCalculator
    {
        private readonly Era era;

        private readonly int minYear;
        private readonly int maxYear;

        internal SingleEraCalculator(Era era, YearMonthDayCalculator ymdCalculator) : base(era)
        {
            minYear = ymdCalculator.MinYear;
            maxYear = ymdCalculator.MaxYear;
            this.era = era;
        }

        private void ValidateEra([NotNull] Era era)
        {
            if (era != this.era)
            {
                Preconditions.CheckNotNull(era, "era");
                Preconditions.CheckArgument(era == this.era, "era", "Only supported era is {0}; requested era was {1}",
                    this.era.Name, era.Name);
            }
        }

        internal override int GetAbsoluteYear(int yearOfEra, Era era)
        {
            ValidateEra(era);
            Preconditions.CheckArgumentRange("yearOfEra", yearOfEra, minYear, maxYear);
            return yearOfEra;
        }

        internal override int GetYearOfEra(YearMonthDay yearMonthDay)
        {
            return yearMonthDay.Year;
        }

        internal override int GetMinYearOfEra(Era era)
        {
            ValidateEra(era);
            return minYear;
        }

        internal override int GetMaxYearOfEra(Era era)
        {
            ValidateEra(era);
            return maxYear;
        }

        internal override Era GetEra(YearMonthDay yearMonthDay)
        {
            return era;
        }
    }
}
