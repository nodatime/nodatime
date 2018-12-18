// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    // All the specific type converters in one place, as they contain no code other than a constructor chaining to the base class.

    internal sealed class AnnualDateTypeConverter : TypeConverterBase<AnnualDate>
    {
        public AnnualDateTypeConverter() : base(AnnualDatePattern.Iso) {}
    }

    internal sealed class DurationTypeConverter : TypeConverterBase<Duration>
    {
        public DurationTypeConverter() : base(DurationPattern.Roundtrip) {}
    }

    internal sealed class InstantTypeConverter : TypeConverterBase<Instant>
    {
        public InstantTypeConverter() : base(InstantPattern.ExtendedIso) {}
    }

    internal sealed class LocalDateTimeTypeConverter : TypeConverterBase<LocalDateTime>
    {
        public LocalDateTimeTypeConverter() : base(LocalDateTimePattern.ExtendedIso) {}
    }

    internal sealed class LocalDateTypeConverter : TypeConverterBase<LocalDate>
    {
        public LocalDateTypeConverter() : base(LocalDatePattern.Iso) {}
    }

    internal sealed class LocalTimeTypeConverter : TypeConverterBase<LocalTime>
    {
        public LocalTimeTypeConverter() : base(LocalTimePattern.ExtendedIso) {}
    }

    internal sealed class PeriodTypeConverter : TypeConverterBase<Period>
    {
        public PeriodTypeConverter() : base(PeriodPattern.Roundtrip) {}
    }
}