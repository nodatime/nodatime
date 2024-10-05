// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET8_0_OR_GREATER
using Microsoft.Extensions.Time.Testing;
using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions;

public class TimeProviderOnlyExtensionsTest
{
    private static readonly DateTimeOffset SampleStartDto = new DateTimeOffset(2024, 10, 5, 11, 29, 0, TimeSpan.FromHours(1));
    private static readonly Instant SampleStartInstant = Instant.FromUtc(2024, 10, 5, 10, 29);

    [Test]
    public void ToClock()
    {
        var provider = new FakeTimeProvider(SampleStartDto);
        var clock = provider.ToClock();
        Assert.AreEqual(SampleStartInstant, clock.GetCurrentInstant());

        provider.Advance(TimeSpan.FromSeconds(1));
        Assert.AreEqual(SampleStartInstant + Duration.FromSeconds(1), clock.GetCurrentInstant());
    }

    [Test]
    public void GetCurrentInstant()
    {
        var provider = new FakeTimeProvider(SampleStartDto);
        Assert.AreEqual(SampleStartInstant, provider.GetCurrentInstant());
    }

    [Test]
    public void ToZonedClock_Iso()
    {
        var provider = new FakeTimeProvider(SampleStartDto);
        // In .NET 8, IANA time zones are always available.
        provider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));

        var zonedClock = provider.ToZonedClock();
        Assert.AreEqual(new LocalDateTime(2024, 10, 5, 11, 29), zonedClock.GetCurrentLocalDateTime());
        Assert.AreEqual(Offset.FromHours(1), zonedClock.GetCurrentOffsetDateTime().Offset);

        // In 2024, Europe/London falls back (to UTC+0) on October 27th at 2am (to 1am)
        provider.Advance(TimeSpan.FromDays(22));
        Assert.AreEqual(new LocalDateTime(2024, 10, 27, 10, 29), zonedClock.GetCurrentLocalDateTime());
        Assert.AreEqual(Offset.Zero, zonedClock.GetCurrentOffsetDateTime().Offset);
    }

    [Test]
    public void ToZonedClock_Julian()
    {
        var calendar = CalendarSystem.Julian;
        var provider = new FakeTimeProvider(SampleStartDto);
        // In .NET 8, IANA time zones are always available.
        provider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));

        var zonedClock = provider.ToZonedClock(CalendarSystem.Julian);
        // 2024-10-05 ISO is 2024-09-22 Julian
        Assert.AreEqual(new LocalDateTime(2024, 9, 22, 11, 29, calendar), zonedClock.GetCurrentLocalDateTime());
        Assert.AreEqual(Offset.FromHours(1), zonedClock.GetCurrentOffsetDateTime().Offset);

        // The time zone is unaffected by which calendar system we use. (So the change on October 27th ISO
        // is a change on October 14th Julian.)
        provider.Advance(TimeSpan.FromDays(22));
        Assert.AreEqual(new LocalDateTime(2024, 10, 14, 10, 29, calendar), zonedClock.GetCurrentLocalDateTime());
        Assert.AreEqual(Offset.Zero, zonedClock.GetCurrentOffsetDateTime().Offset);
    }

    [Test]
    public void ToZonedClock_ZoneChangeAfterConversionIsIgnored()
    {
        var provider = new FakeTimeProvider(SampleStartDto);
        provider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));

        var zonedClock = provider.ToZonedClock();
        provider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));

        // The provider changing to the New York time zone doesn't affect the ZonedClock.
        Assert.AreEqual(new LocalDateTime(2024, 10, 5, 11, 29), zonedClock.GetCurrentLocalDateTime());
    }
}
#endif
