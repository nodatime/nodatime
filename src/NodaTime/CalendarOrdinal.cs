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
        Gregorian = 1,
        Julian = 2,
        Coptic = 3,
        HebrewCivil = 4,
        HebrewScriptural = 5,
        PersianSimple = 6,
        PersianArithmetic = 7,
        PersianAstronomical = 8,
        IslamicAstronomicalBase15 = 9,
        IslamicAstronomicalBase16 = 10,
        IslamicAstronomicalIndian = 11,
        IslamicAstronomicalHabashAlHasib = 12,
        IslamicCivilBase15 = 13,
        IslamicCivilBase16 = 14,
        IslamicCivilIndian = 15,
        IslamicCivilHabashAlHasib = 16,
        UmAlQura = 17,
        Badi = 18,
        // Not a real ordinal; just present to keep a count. Increase this as the number increases...
        Size = 19
    }
}
