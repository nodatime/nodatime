// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class DurationTypeConverterTest : TypeConverterBaseTestBase<Duration>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            DurationPattern.Roundtrip.Format(default),
            DurationPattern.Roundtrip.Format(Duration.Epsilon),
            DurationPattern.Roundtrip.Format(Duration.MaxValue),
            DurationPattern.Roundtrip.Format(Duration.MinValue),
            DurationPattern.Roundtrip.Format(Duration.OneDay),
            DurationPattern.Roundtrip.Format(Duration.OneWeek),
            DurationPattern.Roundtrip.Format(Duration.Zero)
        };
    }
}