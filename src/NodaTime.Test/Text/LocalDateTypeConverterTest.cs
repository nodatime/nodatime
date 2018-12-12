// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class LocalDateTypeConverterTest : TypeConverterBaseTestBase<LocalDate>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            LocalDatePattern.Iso.Format(default),
            LocalDatePattern.Iso.Format(LocalDate.MaxIsoValue),
            LocalDatePattern.Iso.Format(LocalDate.MinIsoValue)
        };
    }
}