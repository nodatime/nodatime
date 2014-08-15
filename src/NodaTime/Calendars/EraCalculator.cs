// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Takes responsibility for all era-based calculations for a calendar.
    /// YearMonthDay arguments can be assumed to be valid for the relevant calendar,
    /// but other arguments should be validated. (Eras should be validated for nullity as well
    /// as for the presence of a particular era.)
    /// </summary>
    internal abstract class EraCalculator
    {
        private readonly IList<Era> eras;

        internal IList<Era> Eras { get { return eras; } }

        protected EraCalculator(params Era[] eras)
        {
            this.eras = new ReadOnlyCollection<Era>(eras);
        }

        internal abstract int GetMinYearOfEra([NotNull] Era era);
        internal abstract int GetMaxYearOfEra([NotNull] Era era);
        internal abstract Era GetEra(YearMonthDay yearMonthDay);
        internal abstract int GetYearOfEra(YearMonthDay yearMonthDay);
        internal abstract int GetAbsoluteYear(int yearOfEra, [NotNull] Era era);
    }
}
