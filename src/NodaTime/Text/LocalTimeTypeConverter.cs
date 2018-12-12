// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <see cref="LocalTime"/> with the <see cref="LocalTimePattern.ExtendedIso"/> pattern.
    /// </summary>
    internal sealed class LocalTimeTypeConverter : TypeConverterBase<LocalTime>
    {
        /// <inheritdoc />
        public LocalTimeTypeConverter() : base(LocalTimePattern.ExtendedIso) {}
    }
}