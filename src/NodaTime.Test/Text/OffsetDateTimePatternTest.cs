// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    public class OffsetDateTimePatternTest : PatternTestBase<OffsetDateTime>
    {
        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns. We've got an offset of 1 hour though.
        private static readonly OffsetDateTime MsdnStandardExample = LocalDateTimePatternTest.MsdnStandardExample.WithOffset(Offset.FromHours(1));
        private static readonly OffsetDateTime MsdnStandardExampleNoMillis = LocalDateTimePatternTest.MsdnStandardExampleNoMillis.WithOffset(Offset.FromHours(1));
        private static readonly OffsetDateTime SampleOffsetDateTimeCoptic = LocalDateTimePatternTest.SampleLocalDateTimeCoptic.WithOffset(Offset.Zero);

        private static readonly Offset AthensOffset = Offset.FromHours(3);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "dd MM yyyy HH:MM:SS", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'M' } },
            // Note incorrect use of "u" (year) instead of "y" (year of era)
            new Data { Pattern = "dd MM uuuu HH:mm:ss gg", Message = Messages.Parse_EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM yyyy HH:mm:ss gg c", Message = Messages.Parse_CalendarAndEra },
            new Data { Pattern = "g", Message = Messages.Parse_UnknownStandardFormat, Parameters = { 'g', typeof(OffsetDateTime) } },
            // Invalid patterns involving embedded values
            new Data { Pattern = "ld<d> yyyy", Message = Messages.Parse_DateFieldAndEmbeddedDate },
            new Data { Pattern = "l<yyyy-MM-dd HH:mm:ss> dd", Message = Messages.Parse_DateFieldAndEmbeddedDate },
            new Data { Pattern = "ld<d> ld<f>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "lt<T> HH", Message = Messages.Parse_TimeFieldAndEmbeddedTime },
            new Data { Pattern = "l<yyyy-MM-dd HH:mm:ss> HH", Message = Messages.Parse_TimeFieldAndEmbeddedTime },
            new Data { Pattern = "lt<T> lt<t>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "ld<d> l<F>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "l<F> ld<d>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "lt<T> l<F>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "l<F> lt<T>", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'l' } },
        };

        internal static Data[] ParseFailureData = {
            // Failures copied from LocalDateTimePatternTest
            new Data { Pattern = "dd MM yyyy HH:mm:ss", Text = "Complete mismatch", Message = Messages.Parse_MismatchedNumber, Parameters = { "dd" }},
            new Data { Pattern = "(c)", Text = "(xxx)", Message = Messages.Parse_NoMatchingCalendarSystem },
            // 24 as an hour is only valid when the time is midnight
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:05", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:01:00", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:01", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00", Template = new LocalDateTime(1970, 1, 1, 0, 0, 5).WithOffset(Offset.Zero), Message = Messages.Parse_InvalidHour24},
            new Data { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0).WithOffset(Offset.Zero), Message = Messages.Parse_InvalidHour24},

            new Data { Pattern = "yyyy-MM-dd HH:mm:ss o<+HH>", Text = "2011-10-19 16:02 +15:00", Message = Messages.Parse_TimeSeparatorMismatch},
        };

        internal static Data[] ParseOnlyData = {
            // Parse-only tests from LocalDateTimeTest.
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "dd MM yyyy", Text = "19 10 2011", Template = new LocalDateTime(2000, 1, 1, 16, 05, 20).WithOffset(Offset.Zero) },
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "HH:mm:ss", Text = "16:05:20", Template = new LocalDateTime(2011, 10, 19, 0, 0, 0).WithOffset(Offset.Zero) },

            // Parsing using the semi-colon "comma dot" specifier
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;fff", Text = "2011-10-19 16:05:20,352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF", Text = "2011-10-19 16:05:20,352" },

            // 24:00 meaning "start of next day"
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:00" },
            new Data(2011, 10, 20, 0, 0, Offset.FromHours(1)) { Pattern = "yyyy-MM-dd HH:mm:ss o<+HH>", Text = "2011-10-19 24:00:00 +01", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0).WithOffset(Offset.FromHours(-5)) },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00" },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24" },
        };

        internal static Data[] FormatOnlyData = {
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "ddd yyyy", Text = "Wed 2011" },
            
            // Our template value has an offset of 0, but the value has an offset of 1... which is ignored by the pattern
            new Data(MsdnStandardExample) { Pattern = "yyyy-MM-dd HH:mm:ss.FF", Text = "2009-06-15 13:45:30.09" }
        };

        internal static Data[] FormatAndParseData = {
            // Copied from LocalDateTimePatternTest
            // Calendar patterns are invariant
            new Data(MsdnStandardExample) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFF o<G>", Text = "(ISO) 2009-06-15T13:45:30.09 +01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "uuuu-MM-dd(c';'o<g>)'T'HH:mm:ss.FFFFFFF", Text = "2009-06-15(ISO;+01)T13:45:30.09", Culture = Cultures.EnUs },
            new Data(SampleOffsetDateTimeCoptic) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFFFF o<G>", Text = "(Coptic) 1976-06-19T21:13:34.123456789 Z", Culture = Cultures.FrFr },
            new Data(SampleOffsetDateTimeCoptic) { Pattern = "uuuu-MM-dd'C'c'T'HH:mm:ss.FFFFFFFFF o<g>", Text = "1976-06-19CCopticT21:13:34.123456789 +00", Culture = Cultures.EnUs },

            // Standard patterns (all invariant)
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "2009-06-15T13:45:30+01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "o", Text = "2009-06-15T13:45:30.09+01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "r", Text = "2009-06-15T13:45:30.09+01 (ISO)", Culture = Cultures.FrFr },

            // Custom embedded patterns (or mixture of custom and standard)
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "ld<yyyy*MM*dd>'X'lt<HH_mm_ss> o<g>", Text = "2015*10*24X11_55_30 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "lt<HH_mm_ss>'Y'ld<yyyy*MM*dd> o<g>", Text = "11_55_30Y2015*10*24 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "l<HH_mm_ss'Y'yyyy*MM*dd> o<g>", Text = "11_55_30Y2015*10*24 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "ld<d>'X'lt<HH_mm_ss> o<g>", Text = "10/24/2015X11_55_30 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "ld<yyyy*MM*dd>'X'lt<T> o<g>", Text = "2015*10*24X11:55:30 +03" },

            // Standard embedded patterns. Short time versions have a seconds value of 0 so they can round-trip.
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "ld<D> lt<r> o<g>", Text = "Saturday, 24 October 2015 11:55:30 +03" },
            new Data(2015, 10, 24, 11, 55, 0, AthensOffset) { Pattern = "l<f> o<g>", Text = "Saturday, 24 October 2015 11:55 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "l<F> o<g>", Text = "Saturday, 24 October 2015 11:55:30 +03" },
            new Data(2015, 10, 24, 11, 55, 0, AthensOffset) { Pattern = "l<g> o<g>", Text = "10/24/2015 11:55 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "l<G> o<g>", Text = "10/24/2015 11:55:30 +03" },

            // Nested embedded patterns
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "l<ld<D> lt<r>> o<g>", Text = "Saturday, 24 October 2015 11:55:30 +03" },
            new Data(2015, 10, 24, 11, 55, 30, AthensOffset) { Pattern = "l<'X'lt<HH_mm_ss>'Y'ld<yyyy*MM*dd>'X'> o<g>", Text = "X11_55_30Y2015*10*24X +03" },

            // Check that unquoted T still works.
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-ddTHH:mm:ss" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        internal sealed class Data : PatternTestData<OffsetDateTime>
        {
            // Default to the start of the year 2000 UTC
            protected override OffsetDateTime DefaultTemplate => OffsetDateTimePattern.DefaultTemplateValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            internal Data(OffsetDateTime value) : base(value)
            {
            }

            internal Data(int year, int month, int day)
                : this(new LocalDateTime(year, month, day, 0, 0).WithOffset(Offset.Zero))
            {
            }

            internal Data(int year, int month, int day, int hour, int minute, Offset offset)
                : this(new LocalDateTime(year, month, day, hour, minute).WithOffset(offset))
            {
            }

            internal Data(int year, int month, int day, int hour, int minute, int second)
                : this(new LocalDateTime(year, month, day, hour, minute, second).WithOffset(Offset.Zero))
            {
            }

            internal Data(int year, int month, int day, int hour, int minute, int second, Offset offset)
                : this(new LocalDateTime(year, month, day, hour, minute, second).WithOffset(offset))
            {
            }

            internal Data(int year, int month, int day, int hour, int minute, int second, int millis)
                : this(new LocalDateTime(year, month, day, hour, minute, second, millis).WithOffset(Offset.Zero))
            {
            }

            internal Data() : this(OffsetDateTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<OffsetDateTime> CreatePattern() =>
                OffsetDateTimePattern.Create(Pattern, Culture, Template);
        }
    }
}
