// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class LocalTimeTypeConverterTest : TypeConverterBaseTestBase<LocalTime>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            LocalTimePattern.ExtendedIso.Format(default),
            LocalTimePattern.ExtendedIso.Format(LocalTime.MaxValue),
            LocalTimePattern.ExtendedIso.Format(LocalTime.Midnight),
            LocalTimePattern.ExtendedIso.Format(LocalTime.MinValue),
            LocalTimePattern.ExtendedIso.Format(LocalTime.Noon)
        };
    }
}