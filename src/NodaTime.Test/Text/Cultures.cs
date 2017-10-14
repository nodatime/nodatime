// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Globalization;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Cultures to use from various tests.
    /// </summary>
    internal static class Cultures
    {
        // Force the cultures to be read-only for tests, to take advantage of caching. Note that on .NET Core,
        // CultureInfo.GetCultures doesn't exist, so we have a big long list of cultures, generated against
        // .NET 4.6.
        internal static readonly IEnumerable<CultureInfo> AllCultures =
#if NETCORE
            new[] {
                "aa-DJ", "aa-ER", "aa-ET", "af-NA", "af-ZA", "agq-CM", "ak-GH", "am-ET",
                "ar-001", "ar-AE", "ar-BH", "ar-DJ", "ar-DZ", "ar-EG", "ar-ER", "ar-IL",
                "ar-IQ", "ar-JO", "ar-KM", "ar-KW", "ar-LB", "ar-LY", "ar-MA", "ar-MR",
                "ar-OM", "ar-PS", "ar-QA", "ar-SA", "ar-SD", "ar-SO", "ar-SS", "ar-SY",
                "ar-TD", "ar-TN", "ar-YE", "arn-CL", "as-IN", "asa-TZ", "ast-ES", "az-Cyrl-AZ",
                "az-Latn-AZ", "ba-RU", "bas-CM", "be-BY", "bem-ZM", "bez-TZ", "bg-BG", "bin-NG",
                "bm-Latn-ML", "bn-BD", "bn-IN", "bo-CN", "bo-IN", "br-FR", "brx-IN", "bs-Cyrl-BA",
                "bs-Latn-BA", "byn-ER", "ca-AD", "ca-ES", "ca-ES-valencia", "ca-FR", "ca-IT", "cgg-UG",
                "chr-Cher-US", "co-FR", "cs-CZ", "cy-GB", "da-DK", "da-GL", "dav-KE", "de-AT",
                "de-BE", "de-CH", "de-DE", "de-LI", "de-LU", "dje-NE", "dsb-DE", "dua-CM",
                "dv-MV", "dyo-SN", "dz-BT", "ebu-KE", "ee-GH", "ee-TG", "el-CY", "el-GR",
                "en-001", "en-029", "en-150", "en-AG", "en-AI", "en-AS", "en-AU", "en-BB",
                "en-BE", "en-BM", "en-BS", "en-BW", "en-BZ", "en-CA", "en-CC", "en-CK",
                "en-CM", "en-CX", "en-DM", "en-ER", "en-FJ", "en-FK", "en-FM", "en-GB",
                "en-GD", "en-GG", "en-GH", "en-GI", "en-GM", "en-GU", "en-GY", "en-HK",
                "en-ID", "en-IE", "en-IM", "en-IN", "en-IO", "en-JE", "en-JM", "en-KE",
                "en-KI", "en-KN", "en-KY", "en-LC", "en-LR", "en-LS", "en-MG", "en-MH",
                "en-MO", "en-MP", "en-MS", "en-MT", "en-MU", "en-MW", "en-MY", "en-NA",
                "en-NF", "en-NG", "en-NR", "en-NU", "en-NZ", "en-PG", "en-PH", "en-PK",
                "en-PN", "en-PR", "en-PW", "en-RW", "en-SB", "en-SC", "en-SD", "en-SG",
                "en-SH", "en-SL", "en-SS", "en-SX", "en-SZ", "en-TC", "en-TK", "en-TO",
                "en-TT", "en-TV", "en-TZ", "en-UG", "en-UM", "en-US", "en-VC", "en-VG",
                "en-VI", "en-VU", "en-WS", "en-ZA", "en-ZM", "en-ZW", "eo-001", "es-419",
                "es-AR", "es-BO", "es-CL", "es-CO", "es-CR", "es-CU", "es-DO", "es-EC",
                "es-ES", "es-GQ", "es-GT", "es-HN", "es-MX", "es-NI", "es-PA", "es-PE",
                "es-PH", "es-PR", "es-PY", "es-SV", "es-US", "es-UY", "es-VE", "et-EE",
                "eu-ES", "ewo-CM", "fa-IR", "ff-CM", "ff-GN", "ff-Latn-SN", "ff-MR", "ff-NG",
                "fi-FI", "fil-PH", "fo-FO", "fr-029", "fr-BE", "fr-BF", "fr-BI", "fr-BJ",
                "fr-BL", "fr-CA", "fr-CD", "fr-CF", "fr-CG", "fr-CH", "fr-CI", "fr-CM",
                "fr-DJ", "fr-DZ", "fr-FR", "fr-GA", "fr-GF", "fr-GN", "fr-GP", "fr-GQ",
                "fr-HT", "fr-KM", "fr-LU", "fr-MA", "fr-MC", "fr-MF", "fr-MG", "fr-ML",
                "fr-MQ", "fr-MR", "fr-MU", "fr-NC", "fr-NE", "fr-PF", "fr-PM", "fr-RE",
                "fr-RW", "fr-SC", "fr-SN", "fr-SY", "fr-TD", "fr-TG", "fr-TN", "fr-VU",
                "fr-WF", "fr-YT", "fur-IT", "fy-NL", "ga-IE", "gd-GB", "gl-ES", "gn-PY",
                "gsw-CH", "gsw-FR", "gsw-LI", "gu-IN", "guz-KE", "gv-IM", "ha-Latn-GH", "ha-Latn-NE",
                "ha-Latn-NG", "haw-US", "he-IL", "hi-IN", "hr-BA", "hr-HR", "hsb-DE", "hu-HU",
                "hy-AM", "ia-001", "ia-FR", "ibb-NG", "id-ID", "ig-NG", "ii-CN", "is-IS",
                "it-CH", "it-IT", "it-SM", "iu-Cans-CA", "iu-Latn-CA", "ja-JP", "jgo-CM", "jmc-TZ",
                "jv-Java-ID", "jv-Latn-ID", "ka-GE", "kab-DZ", "kam-KE", "kde-TZ", "kea-CV", "khq-ML",
                "ki-KE", "kk-KZ", "kkj-CM", "kl-GL", "kln-KE", "km-KH", "kn-IN", "ko-KR",
                "kok-IN", "kr-NG", "ks-Arab-IN", "ks-Deva-IN", "ksb-TZ", "ksf-CM", "ksh-DE", "ku-Arab-IQ",
                "kw-GB", "ky-KG", "la-001", "lag-TZ", "lb-LU", "lg-UG", "lkt-US", "ln-AO",
                "ln-CD", "ln-CF", "ln-CG", "lo-LA", "lt-LT", "lu-CD", "luo-KE", "luy-KE",
                "lv-LV", "mas-KE", "mas-TZ", "mer-KE", "mfe-MU", "mg-MG", "mgh-MZ", "mgo-CM",
                "mi-NZ", "mk-MK", "ml-IN", "mn-MN", "mn-Mong-CN", "mn-Mong-MN", "mni-IN", "moh-CA",
                "mr-IN", "ms-BN", "ms-MY", "ms-SG", "mt-MT", "mua-CM", "my-MM", "naq-NA",
                "nb-NO", "nb-SJ", "nd-ZW", "ne-IN", "ne-NP", "nl-AW", "nl-BE", "nl-BQ",
                "nl-CW", "nl-NL", "nl-SR", "nl-SX", "nmg-CM", "nn-NO", "nnh-CM", "nqo-GN",
                "nr-ZA", "nso-ZA", "nus-SS", "nyn-UG", "oc-FR", "om-ET", "om-KE", "or-IN",
                "os-GE", "os-RU", "pa-Arab-PK", "pa-IN", "pap-029", "pl-PL", "prs-AF", "ps-AF",
                "pt-AO", "pt-BR", "pt-CV", "pt-GW", "pt-MO", "pt-MZ", "pt-PT", "pt-ST",
                "pt-TL", "quc-Latn-GT", "quz-BO", "quz-EC", "quz-PE", "rm-CH", "rn-BI", "ro-MD",
                "ro-RO", "rof-TZ", "ru-BY", "ru-KG", "ru-KZ", "ru-MD", "ru-RU", "ru-UA",
                "rw-RW", "rwk-TZ", "sa-IN", "sah-RU", "saq-KE", "sbp-TZ", "sd-Arab-PK", "sd-Deva-IN",
                "se-FI", "se-NO", "se-SE", "seh-MZ", "ses-ML", "sg-CF", "shi-Latn-MA", "shi-Tfng-MA",
                "si-LK", "sk-SK", "sl-SI", "sma-NO", "sma-SE", "smj-NO", "smj-SE", "smn-FI",
                "sms-FI", "sn-Latn-ZW", "so-DJ", "so-ET", "so-KE", "so-SO", "sq-AL", "sq-MK",
                "sq-XK", "sr-Cyrl-BA", "sr-Cyrl-ME", "sr-Cyrl-RS", "sr-Cyrl-XK", "sr-Latn-BA", "sr-Latn-ME", "sr-Latn-RS",
                "sr-Latn-XK", "ss-SZ", "ss-ZA", "ssy-ER", "st-LS", "st-ZA", "sv-AX", "sv-FI",
                "sv-SE", "sw-CD", "sw-KE", "sw-TZ", "sw-UG", "swc-CD", "syr-SY", "ta-IN",
                "ta-LK", "ta-MY", "ta-SG", "te-IN", "teo-KE", "teo-UG", "tg-Cyrl-TJ", "th-TH",
                "ti-ER", "ti-ET", "tig-ER", "tk-TM", "tn-BW", "tn-ZA", "to-TO", "tr-CY",
                "tr-TR", "ts-ZA", "tt-RU", "twq-NE", "tzm-Arab-MA", "tzm-Latn-DZ", "tzm-Latn-MA", "tzm-Tfng-MA",
                "ug-CN", "uk-UA", "ur-IN", "ur-PK", "uz-Arab-AF", "uz-Cyrl-UZ", "uz-Latn-UZ", "vai-Latn-LR",
                "vai-Vaii-LR", "ve-ZA", "vi-VN", "vo-001", "vun-TZ", "wae-CH", "wal-ET", "wo-SN",
                "xh-ZA", "xog-UG", "yav-CM", "yi-001", "yo-BJ", "yo-NG", "zgh-Tfng-MA", "zh-CN"
            }.Select(name => new CultureInfo(name))
#else
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
#endif
            .Where(culture => !RuntimeFailsToLookupResourcesForCulture(culture))
            .Where(culture => !MonthNamesCompareEqual(culture))
            .Select(CultureInfo.ReadOnly)
            .ToList();

        internal static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        // Specify en-US patterns explicitly, as .NET Core on Linux gives a different answer. We
        // don't need it to be US English really, just an example...

        internal static readonly CultureInfo EnUs = CultureInfo.ReadOnly(new CultureInfo("en-US")
        {
            DateTimeFormat =
            {
                LongDatePattern = "dddd, MMMM d, yyyy",
                LongTimePattern = "h:mm:ss tt",
                ShortDatePattern = "M/d/yyyy",
                ShortTimePattern = "h:mm tt"
            }
        });
        // Specify fr-FR patterns explicitly, as .NET Core on Linux has different information.
        // We don't need it to be French really, just an example...
        internal static readonly CultureInfo FrFr = CultureInfo.ReadOnly(new CultureInfo("fr-FR")
        {
            DateTimeFormat =
            {
                LongDatePattern = "dddd d MMMM yyyy",
                LongTimePattern = "HH:mm:ss",
                ShortDatePattern = "dd/MM/yyyy",
                ShortTimePattern = "HH:mm",
                AbbreviatedDayNames = LowerCaseFrench(dtf => dtf.AbbreviatedDayNames),
                AbbreviatedMonthNames = LowerCaseFrench(dtf => dtf.AbbreviatedMonthNames),
                DayNames = LowerCaseFrench(dtf => dtf.DayNames),
                MonthNames = LowerCaseFrench(dtf => dtf.MonthNames),
                MonthGenitiveNames = LowerCaseFrench(dtf => dtf.MonthGenitiveNames),
            }
        });
        internal static readonly CultureInfo FrCa = CultureInfo.ReadOnly(new CultureInfo("fr-CA")
        {
            DateTimeFormat =
            {
                LongDatePattern = "d MMMM yyyy",
                LongTimePattern = "HH:mm:ss",
                ShortDatePattern = "yyyy-MM-dd",
                ShortTimePattern = "HH:mm"
            }
        });

        private static string[] LowerCaseFrench(Func<DateTimeFormatInfo, string[]> propertySelector) =>
            propertySelector(new CultureInfo("fr-FR").DateTimeFormat).Select(x => x?.ToLowerInvariant()).ToArray();

        // We need a culture with a non-colon time separator. On .NET Core we can't specify this explicitly,
        // so we just rely on Finland doing the right thing. On platforms where we can set this explicitly,
        // we do so - still starting with Finland for consistency.
#if NETCORE
        internal static readonly CultureInfo DotTimeSeparator = CultureInfo.ReadOnly(new CultureInfo("fi-FI"));
#else
        internal static readonly CultureInfo DotTimeSeparator = CultureInfo.ReadOnly(new CultureInfo("fi-FI") {
            DateTimeFormat = { TimeSeparator = "." }
        });
#endif

        internal static readonly CultureInfo GenitiveNameTestCulture = CreateGenitiveTestCulture();
        internal static readonly CultureInfo GenitiveNameTestCultureWithLeadingNames = CreateGenitiveTestCultureWithLeadingNames();
        internal static readonly CultureInfo AwkwardDayOfWeekCulture = CreateAwkwardDayOfWeekCulture();
        internal static readonly CultureInfo AwkwardAmPmDesignatorCulture = CreateAwkwardAmPmCulture();

        /// <summary>
        /// Workaround for the lack of CultureInfo.GetCultureInfo(string) in netcoreapp1.0.
        /// Our .NET Core version doesn't cache, but does at least still return a readonly version.
        /// </summary>
        internal static CultureInfo GetCultureInfo(string name)
        {
#if NETCORE
            return CultureInfo.ReadOnly(new CultureInfo(name));
#else
            return CultureInfo.GetCultureInfo(name);
#endif
        }

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

        /// <summary>
        /// Creates a culture where the AM designator is a substring of the PM designator.
        /// </summary>
        private static CultureInfo CreateAwkwardAmPmCulture()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            format.AMDesignator = "Foo";
            format.PMDesignator = "FooBar";
            return clone;
        }

        private static string[] ReplaceFirstElement(string[] input, string newElement)
        {
            // Cloning so we don't accidentally *really* change any read-only cultures, to work around a bug in Mono.
            string[] clone = (string[])input.Clone();
            clone[0] = newElement;
            return clone;
        }

        /// <summary>
        /// Tests whether it's feasible to tell the difference between all the months in the year,
        /// using the culture's compare info. On Mono, text comparison is broken for some cultures.
        /// In those cultures, it simply doesn't make sense to run the tests.
        /// See https://bugzilla.xamarin.com/show_bug.cgi?id=11366 for more info.
        /// </summary>
        private static bool MonthNamesCompareEqual(CultureInfo culture)
        {
            var months = culture.DateTimeFormat.MonthNames;
            var compareInfo = culture.CompareInfo;
            for (int i = 0; i < months.Length - 1; i++)
            {
                for (int j = i + 1; j < months.Length; j++)
                {
                    if (compareInfo.Compare(months[i], months[j], CompareOptions.IgnoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tests whether the runtime will be able to look up resources for the given culture. Mono 3.0 appears to have
        /// some problems with fallback for zh-CN and zh-SG, so we suppress tests against those cultures.
        /// See https://bugzilla.xamarin.com/show_bug.cgi?id=11375
        /// </summary>
        private static bool RuntimeFailsToLookupResourcesForCulture(CultureInfo culture)
        {
            try
            {
                PatternResources.ResourceManager.GetString("OffsetPatternLong", culture);
            }
            catch (ArgumentNullException)
            {
                return true;
            }
            return false;
        }
    }
}
