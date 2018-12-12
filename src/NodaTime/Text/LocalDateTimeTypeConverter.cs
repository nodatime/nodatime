// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <see cref="LocalDateTime"/> with the <see cref="LocalDateTimePattern.ExtendedIso"/> pattern.
    /// </summary>
    internal sealed class LocalDateTimeTypeConverter : TypeConverterBase<LocalDateTime>
    {
        /// <inheritdoc />
        private LocalDateTimeTypeConverter() : base(LocalDateTimePattern.ExtendedIso) {}
    }
}