// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Cultures to use from various tests.
    /// </summary>
    internal static class Cultures
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        // Force the cultures to be read-only for tests, to take advantage of caching. Our Continuous Integration system
        // is very slow at reading resources (in the NodaFormatInfo constructor).
        // Note: R# suggests using a method group conversion for the Select call here, which is fine with the C# 4 compiler,
        // but doesn't work with the C# 3 compiler (which doesn't have quite as good type inference).
        internal static readonly IEnumerable<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => CultureInfo.ReadOnly(culture)).ToList();
        // Some tests don't run nicely on Mono, e.g. as they have characters we don't expect in their long/short patterns.
        // Pretend we have no cultures, for the sake of these tests.
        // TODO(V1.2): Make the tests pass instead?
        internal static readonly IEnumerable<CultureInfo> AllCulturesOrEmptyOnMono = TestHelper.IsRunningOnMono ? new CultureInfo[0] : Cultures.AllCultures;
#pragma warning restore 0414

        internal static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        internal static readonly CultureInfo EnUs = CultureInfo.ReadOnly(new CultureInfo("en-US"));
        internal static readonly CultureInfo FrFr = CultureInfo.ReadOnly(new CultureInfo("fr-FR"));
        internal static readonly CultureInfo FrCa = CultureInfo.ReadOnly(new CultureInfo("fr-CA"));

        // We mostly use Italy as an example of a culture with a "." as the time separator
        // - but it doesn't have it on Mono, so force it here. (In fact, it looks like it
        // changed to : between .NET 2 and .NET 4 anyway... another reason to force it.)
        internal static readonly CultureInfo ItIt = CultureInfo.ReadOnly(new CultureInfo("it-IT") {
            DateTimeFormat = { TimeSeparator = "." }
        });
        
        internal static readonly CultureInfo GenitiveNameTestCulture = CreateGenitiveTestCulture();
        internal static readonly CultureInfo GenitiveNameTestCultureWithLeadingNames = CreateGenitiveTestCultureWithLeadingNames();
        internal static readonly CultureInfo AwkwardDayOfWeekCulture = CreateAwkwardDayOfWeekCulture();

        /// <summary>
        /// .NET 3.5 doesn't contain any cultures where the abbreviated month names differ
        /// from the non-abbreviated month names. As we're testing under .NET 3.5, we'll need to create
        /// our own. This is just a clone of the invarant culture, with month 1 changed.
        /// </summary>
        private static CultureInfo CreateGenitiveTestCulture()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            format.MonthNames = ReplaceFirstElement(format.MonthNames, "FullNonGenName");
            format.MonthGenitiveNames = ReplaceFirstElement(format.MonthGenitiveNames, "FullGenName");
            format.AbbreviatedMonthNames = ReplaceFirstElement(format.AbbreviatedMonthNames, "AbbrNonGenName");
            format.AbbreviatedMonthGenitiveNames = ReplaceFirstElement(format.AbbreviatedMonthGenitiveNames, "AbbrGenName");
            // Note: Do *not* use CultureInfo.ReadOnly here, as it's broken in .NET 3.5 and lower; the month names
            // get reset to the original values, making the customization pointless :(
            return clone;
        }

        /// <summary>
        /// Some cultures may have genitive month names which are longer than the non-genitive names.
        /// On Windows, we could use dsb-DE for this - but that doesn't exist in Mono.
        /// </summary>
        private static CultureInfo CreateGenitiveTestCultureWithLeadingNames()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            format.MonthNames = ReplaceFirstElement(format.MonthNames, "MonthName");
            format.MonthGenitiveNames = ReplaceFirstElement(format.MonthGenitiveNames, "MonthName-Genitive");
            format.AbbreviatedMonthNames = ReplaceFirstElement(format.AbbreviatedMonthNames, "MN");
            format.AbbreviatedMonthGenitiveNames = ReplaceFirstElement(format.AbbreviatedMonthGenitiveNames, "MN-Gen");
            // Note: Do *not* use CultureInfo.ReadOnly here, as it's broken in .NET 3.5 and lower; the month names
            // get reset to the original values, making the customization pointless :(
            return clone;
        }

        /// <summary>
        /// Like the invariant culture, but Thursday is called "FooBa" (or "Foobaz" for short, despite being longer), and
        /// Friday is called "FooBar" (or "Foo" for short). This is expected to confuse our parser if we're not careful.
        /// </summary>
        /// <returns></returns>
        private static CultureInfo CreateAwkwardDayOfWeekCulture()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            string[] longDayNames = format.DayNames;
            string[] shortDayNames = format.AbbreviatedDayNames;
            longDayNames[(int)DayOfWeek.Thursday] = "FooBa";
            shortDayNames[(int)DayOfWeek.Thursday] = "FooBaz";
            longDayNames[(int)DayOfWeek.Friday] = "FooBar";
            shortDayNames[(int)DayOfWeek.Friday] = "Foo";
            format.DayNames = longDayNames;
            format.AbbreviatedDayNames = shortDayNames;
            return clone;
        }

        private static string[] ReplaceFirstElement(string[] input, string newElement)
        {
            // Cloning so we don't accidentally *really* change any read-only cultures, to work around a bug in Mono.
            string[] clone = (string[])input.Clone();
            clone[0] = newElement;
            return clone;
        }
    }
}
