// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime
{
    /// <summary>
    /// Enumeration of calendar ordinal values. Used for converting between a compact integer representation and a calendar system.
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
        Persian = 12,
        IslamicAstronomicalBase15 = 13,
        IslamicAstronomicalBase16 = 14,
        IslamicAstronomicalIndian = 15,
        IslamicAstronomicalHabashAlHasib = 16,
        IslamicCivilBase15 = 17,
        IslamicCivilBase16 = 18,
        IslamicCivilIndian = 19,
        IslamicCivilHabashAlHasib = 20,
        UmAlQura = 21,
        // Not a real ordinal; just present to keep a count. Increase this as the number increases...
        Size = 22
    }
}
