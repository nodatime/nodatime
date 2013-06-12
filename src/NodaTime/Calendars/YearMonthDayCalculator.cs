// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Calendars
{
    internal abstract class YearMonthDayCalculator
    {
        private readonly int minYear;
        internal int MinYear { get { return minYear; } }

        private readonly int maxYear;
        internal int MaxYear { get { return maxYear; } }

        public IEnumerable<Era> Eras
        {
            get { throw new NotImplementedException(); }
        }

        internal LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth)
        {
            throw new NotImplementedException();
        }

        internal LocalInstant GetLocalInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            throw new NotImplementedException();
        }

        internal int GetDaysInMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        internal bool IsLeapYear(int year)
        {
            throw new NotImplementedException();
        }

        internal int GetMaxMonth(int year)
        {
            throw new NotImplementedException();
        }
    }
}
