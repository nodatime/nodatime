// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;
using System.Globalization;

namespace NodaTime.Test.Calendars;

/// <summary>
/// Tests for PersianYearMonthDayCalculator via the Persian CalendarSystem.
/// </summary>
public class PersianCalendarSystemTest
{
    [Test]
    [Explicit]
    [Category("Slow")]
    public void BclThroughHistory()
    {
        Calendar bcl = BclCalendars.Persian;
        CalendarSystem noda = BclCalendars.CalendarSystemForCalendar(bcl)!;
        // Note: Noda Time stops in 9377, whereas the BCL goes into the start of 9378. This is because
        // Noda Time ensures that the whole year is valid.
        BclEquivalenceHelper.AssertEquivalent(bcl, noda);
    }

    /// <summary>
    /// Use the examples in Calendrical Calculations for where the arithmetic calendar differs
    /// from the astronomical one.
    /// </summary>
    [Test]
    [TestCase(1016, 1637, 21)]
    [TestCase(1049, 1670, 21)]
    [TestCase(1078, 1699, 21)]
    [TestCase(1082, 1703, 22)]
    [TestCase(1111, 1732, 21)]
    [TestCase(1115, 1736, 21)]
    [TestCase(1144, 1765, 21)]
    [TestCase(1177, 1798, 21)]
    [TestCase(1210, 1831, 22)]
    [TestCase(1243, 1864, 21)]
    [TestCase(1404, 2025, 20)]
    [TestCase(1437, 2058, 20)]
    [TestCase(1532, 2153, 20)]
    [TestCase(1565, 2186, 20)]
    [TestCase(1569, 2190, 20)]
    [TestCase(1598, 2219, 21)]
    [TestCase(1631, 2252, 20)]
    [TestCase(1660, 2281, 20)]
    [TestCase(1664, 2285, 20)]
    [TestCase(1693, 2314, 21)]
    [TestCase(1697, 2318, 21)]
    [TestCase(1726, 2347, 21)]
    [TestCase(1730, 2351, 21)]
    [TestCase(1759, 2380, 20)]
    [TestCase(1763, 2384, 20)]
    [TestCase(1788, 2409, 20)]
    [TestCase(1792, 2413, 20)]
    [TestCase(1796, 2417, 20)]

    public void ArithmeticExamples(int persianYear, int gregorianYear, int gregorianDayOfMarch)
    {
        var persian = new LocalDate(persianYear, 1, 1, CalendarSystem.PersianArithmetic);
        var gregorian = persian.WithCalendar(CalendarSystem.Gregorian);
        Assert.AreEqual(gregorianYear, gregorian.Year);
        Assert.AreEqual(3, gregorian.Month);
        Assert.AreEqual(gregorianDayOfMarch, gregorian.Day);
    }

    /// <summary>
    /// The arithmetic calendar repeats every 2820 years, including the years before 474.
    /// A floored modulo puts year 24 in the same position in the cycle as year 2844.
    /// </summary>
    [Test]
    public void ArithmeticLeapYearsRepeatEvery2820Years()
    {
        var calendar = CalendarSystem.PersianArithmetic;
        for (var year = calendar.MinYear; year + 2820 <= calendar.MaxYear; year++)
        {
            Assert.AreEqual(calendar.IsLeapYear(year + 2820), calendar.IsLeapYear(year), "Year {0}", year);
        }
    }

    /// <summary>
    /// Leap years and new year's days before 474 use the same cycle as later years.
    /// </summary>
    [Test]
    [TestCase(24, false, 645, 21)]
    [TestCase(25, true, 646, 21)]
    [TestCase(57, false, 678, 21)]
    [TestCase(58, true, 679, 21)]
    [TestCase(441, false, 1062, 21)]
    [TestCase(442, true, 1063, 21)]
    public void ArithmeticYearsBefore474(int persianYear, bool leap, int gregorianYear, int gregorianDayOfMarch)
    {
        Assert.AreEqual(leap, CalendarSystem.PersianArithmetic.IsLeapYear(persianYear));
        var gregorian = new LocalDate(persianYear, 1, 1, CalendarSystem.PersianArithmetic).WithCalendar(CalendarSystem.Gregorian);
        Assert.AreEqual(new LocalDate(gregorianYear, 3, gregorianDayOfMarch, CalendarSystem.Gregorian), gregorian);
    }

    [Test]
    public void ArithmeticYearEndBefore474()
    {
        var calendar = CalendarSystem.PersianArithmetic;
        var lastDayOfYear24 = new LocalDate(24, 12, 29, calendar);
        Assert.AreEqual(new LocalDate(25, 1, 1, calendar), lastDayOfYear24.PlusDays(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(24, 12, 30, calendar));

        var leapDayOfYear25 = new LocalDate(25, 12, 30, calendar);
        Assert.AreEqual(new LocalDate(26, 1, 1, calendar), leapDayOfYear25.PlusDays(1));
    }

#if DEBUG
    // Only a test to make it easy to generate the data.
    [Test, Explicit]
    public void GenerateData()
    {
        var maxYear = PersianYearMonthDayCalculator.MaxPersianYear;
        var bcl = new PersianCalendar();
        byte[] data = new byte[maxYear / 8 + 1];
        // We don't really care whether IsLeapYear(MaxPersianYear+1) returns true or false,
        // but it must be valid to call it.
        for (int year = 1; year <= maxYear; year++)
        {
            if (bcl.IsLeapYear(year))
            {
                data[year >> 3] |= (byte)(1 << (year & 7));
            }
        }
        var base64 = Convert.ToBase64String(data);
        var lineLength = 80;
        for (int start = 0; start < base64.Length; start += lineLength)
        {
            var line = base64.Substring(start, Math.Min(lineLength, base64.Length - start));
            var last = start + lineLength >= base64.Length;
            Console.WriteLine($"\"{line}\"{(last ? "" : " +")}");
        }
    }
#endif
}
