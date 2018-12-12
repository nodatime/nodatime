// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <see cref="AnnualDate"/> with the <see cref="AnnualDatePattern.Iso"/> pattern.
    /// </summary>
    internal sealed class AnnualDateTypeConverter : TypeConverterBase<AnnualDate>
    {
        /// <inheritdoc />
        public AnnualDateTypeConverter() : base(AnnualDatePattern.Iso) {}
    }
}