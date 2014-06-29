﻿// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calculator to handle time-of-day related fields.
    /// This is a static class because we don't intend to model
    /// different lengths of days, or hours etc. (We have no state
    /// at all, and need no polymorphism.)
    /// </summary>
    internal static class TimeOfDayCalculator
    {
        internal static int GetHourOfHalfDayFromTickOfDay(long tickOfDay)
        {
            return GetHourOfDayFromTickOfDay(tickOfDay) % 12;
        }

        internal static int GetClockHourOfHalfDayFromTickOfDay(long tickOfDay)
        {
            int hourOfHalfDay = GetHourOfHalfDayFromTickOfDay(tickOfDay);
            return hourOfHalfDay == 0 ? 12 : hourOfHalfDay;
        }

        internal static int GetHourOfDayFromTickOfDay(long tickOfDay)
        {
            // Effectively tickOfDay / NodaConstants.TicksPerHour.
            // Note that NodaConstants.TicksPerStandardDay >> 11 is about 491 million; less than int.MaxValue.
            return ((int) (tickOfDay >> 11)) / 17578125;
        }

        internal static int GetMinuteOfHourFromTickOfDay(long tickOfDay)
        {
            int minuteOfDay = (int) (tickOfDay / (int) NodaConstants.TicksPerMinute);
            return minuteOfDay % NodaConstants.MinutesPerHour;
        }

        internal static int GetSecondOfMinuteFromTickOfDay(long tickOfDay)
        {
            int secondOfDay = (int) (tickOfDay / (int) NodaConstants.TicksPerSecond);
            return secondOfDay % NodaConstants.SecondsPerMinute;
        }

        internal static int GetMillisecondOfSecondFromTickOfDay(long tickOfDay)
        {
            long milliSecondOfDay = (tickOfDay / (int) NodaConstants.TicksPerMillisecond);
            return (int) (milliSecondOfDay % NodaConstants.MillisecondsPerSecond);
        }

        internal static int GetTickOfSecondFromTickOfDay(long tickOfDay)
        {
            return (int) (tickOfDay % (int) NodaConstants.TicksPerSecond);
        }
    }
}
