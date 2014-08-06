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

        private readonly YearMonthDay startOfEra;
        private readonly YearMonthDay endOfEra;

        internal SingleEraCalculator(Era era, YearMonthDayCalculator ymdCalculator) : base(era)
        {
            int minDays = ymdCalculator.GetStartOfYearInDays(ymdCalculator.MinYear);
            int maxDays = ymdCalculator.GetStartOfYearInDays(ymdCalculator.MaxYear) + ymdCalculator.GetDaysInYear(ymdCalculator.MaxYear) - 1;
            startOfEra = ymdCalculator.GetYearMonthDay(minDays);
            endOfEra = ymdCalculator.GetYearMonthDay(maxDays);
            this.era = era;
        }

        private void ValidateEra([NotNull] Era era)
        {
            Preconditions.CheckNotNull(era, "era");
            if (era != this.era)
            {
                Preconditions.CheckArgument(era == this.era, "era", "Only supported era is {0}; requested era was {1}",
                    this.era.Name, era.Name);
            }
        }

        internal override int GetAbsoluteYear(int yearOfEra, Era era)
        {
            ValidateEra(era);
            Preconditions.CheckArgumentRange("yearOfEra", yearOfEra, startOfEra.Year, endOfEra.Year);
            return yearOfEra;
        }

        internal override int GetYearOfEra(YearMonthDay yearMonthDay)
        {
            return yearMonthDay.Year;
        }

        internal override int GetMinYearOfEra(Era era)
        {
            ValidateEra(era);
            return startOfEra.Year;
        }

        internal override int GetMaxYearOfEra(Era era)
        {
            ValidateEra(era);
            return endOfEra.Year;
        }

        internal override int GetCenturyOfEra(YearMonthDay yearMonthDay)
        {
            int yearOfEra = yearMonthDay.Year;
            int zeroBasedRemainder = yearOfEra % 100;
            int zeroBasedResult = yearOfEra / 100;
            return zeroBasedRemainder == 0 ? zeroBasedResult : zeroBasedResult + 1;
        }

        internal override int GetYearOfCentury(YearMonthDay yearMonthDay)
        {
            int zeroBased = yearMonthDay.Year % 100;
            return zeroBased == 0 ? 100 : zeroBased;
        }

        internal override YearMonthDay GetStartOfEra(Era era)
        {
            ValidateEra(era);
            return startOfEra;
        }

        internal override YearMonthDay GetEndOfEra(Era era)
        {
            ValidateEra(era);
            return endOfEra;
        }

        internal override Era GetEra(YearMonthDay yearMonthDay)
        {
            return era;
        }
    }
}
