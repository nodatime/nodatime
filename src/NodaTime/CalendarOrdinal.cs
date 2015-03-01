// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime
{
    /// <summary>
    /// Enumeration of calendar ordinal values. Used for converting between a compact integer representation and a calendar system.
    /// We use 7 bits to store the calendar ordinal in YearMonthDayCalendar, so we can have up to 128 calendars.
    /// </summary>
    internal enum CalendarOrdinal
    {
        Iso = 0,
        Gregorian1 = 1,
        Gregorian2 = 2,
        Gregorian3 = 3,
        Gregorian4 = 4,
        Gregorian5 = 5,
        Gregorian6 = 6,
        Gregorian7 = 7,
        Julian = 8,
        Coptic = 9,
        HebrewCivil = 10,
        HebrewScriptural = 11,
        PersianSimple = 12,
        PersianArithmetic = 13,
        PersianAstronomical = 14,
        IslamicAstronomicalBase15 = 15,
        IslamicAstronomicalBase16 = 16,
        IslamicAstronomicalIndian = 17,
        IslamicAstronomicalHabashAlHasib = 18,
        IslamicCivilBase15 = 19,
        IslamicCivilBase16 = 20,
        IslamicCivilIndian = 21,
        IslamicCivilHabashAlHasib = 22,
        UmAlQura = 23,
        // Not a real ordinal; just present to keep a count. Increase this as the number increases...
        Size = 24
    }
}
