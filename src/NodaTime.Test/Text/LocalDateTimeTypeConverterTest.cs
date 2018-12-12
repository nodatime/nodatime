// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class LocalDateTimeTypeConverterTest : TypeConverterBaseTestBase<LocalDateTime>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            LocalDateTimePattern.ExtendedIso.Format(default(LocalDateTime)),
            LocalDateTimePattern.ExtendedIso.Format(new LocalDateTime(2018, 01, 01, 00, 00, 00)),
            LocalDateTimePattern.ExtendedIso.Format(new LocalDateTime(2018, 12, 31, 23, 59, 59))
        };
    }
}