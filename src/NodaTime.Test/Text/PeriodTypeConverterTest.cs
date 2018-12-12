// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class PeriodTypeConverterTest : TypeConverterBaseTestBase<Period>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            PeriodPattern.Roundtrip.Format(Period.FromDays(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromDays(1)),
            PeriodPattern.Roundtrip.Format(Period.FromHours(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromHours(1)),
            PeriodPattern.Roundtrip.Format(Period.FromMilliseconds(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromMilliseconds(1)),
            PeriodPattern.Roundtrip.Format(Period.FromMinutes(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromMinutes(1)),
            PeriodPattern.Roundtrip.Format(Period.FromMonths(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromMonths(1)),
            PeriodPattern.Roundtrip.Format(Period.FromNanoseconds(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromNanoseconds(1)),
            PeriodPattern.Roundtrip.Format(Period.FromSeconds(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromSeconds(1)),
            PeriodPattern.Roundtrip.Format(Period.FromTicks(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromTicks(1)),
            PeriodPattern.Roundtrip.Format(Period.FromWeeks(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromWeeks(1)),
            PeriodPattern.Roundtrip.Format(Period.FromYears(-1)),
            PeriodPattern.Roundtrip.Format(Period.FromYears(1)),
            PeriodPattern.Roundtrip.Format(Period.Zero)
        };
    }
}