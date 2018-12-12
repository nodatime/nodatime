// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [UsedImplicitly]
    public class AnnualDateTypeConverterTest : TypeConverterBaseTestBase<AnnualDate>
    {
        [NotNull] [UsedImplicitly] static readonly string[] RoundtripData =
        {
            AnnualDatePattern.Iso.Format(default),
            AnnualDatePattern.Iso.Format(new AnnualDate(01, 01)),
            AnnualDatePattern.Iso.Format(new AnnualDate(12, 31))
        };
    }
}