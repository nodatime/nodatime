// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <see cref="Period"/> with the <see cref="PeriodPattern.Roundtrip"/> pattern.
    /// </summary>
    internal sealed class PeriodTypeConverter : TypeConverterBase<Period>
    {
        /// <inheritdoc />
        public PeriodTypeConverter() : base(PeriodPattern.Roundtrip) {}
    }
}