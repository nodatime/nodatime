// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <see cref="Duration"/> with the <see cref="DurationPattern.Roundtrip"/> pattern.
    /// </summary>
    internal sealed class DurationTypeConverter : TypeConverterBase<Duration>
    {
        /// <inheritdoc />
        public DurationTypeConverter() : base(DurationPattern.Roundtrip) {}
    }
}