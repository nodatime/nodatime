---
layout: userguide
title: Core types quick reference
category: core
weight: 1030
---

This is a companion page to the
["core concepts"](concepts.html), and ["choosing between types"](type-choices.html)
pages, describing the fundamental types in Noda Time very briefly, primarily for reference.
If you're not familiar with the core concepts, read that page first.

Instant
-------

An [`Instant`][Instant] is a point on notional a global time-line, regardless of calendar system and time zone.
It's simply a number of "ticks" since some arbitrary epoch, where a tick in Noda Time is 100 nanoseconds (a definition
inherited from the BCL). Noda Time always uses the Unix epoch, which corresponds to midnight on January 1st 1970 UTC.
(This is merely one way of expressing the epoch - it would be equally valid to express it using other calendar systems
and time zones; the epoch itself has no notion of a time zone or calendar system.)

Offset
------

An [`Offset`][Offset] is used to express the difference between UTC and local time. It is always *added* to a UTC value
to obtain a local time, or *subtracted* from a local time to obtain a UTC value.

CalendarSystem
--------------

A [`CalendarSystem`][CalendarSystem] is a way of dividing up a time line into human-friendly units - minutes, hours, days, months, years
and so forth. The "default" calendar system in Noda Time is the ISO-8601 calendar, which is basically the Gregorian calendar.
Other calendar systems are available, including Julian and Islamic calendars.

A calendar system is orthogonal to a time zone - a time zone effectively just offsets the global time line.

LocalDateTime
-------------

A [`LocalDateTime`][LocalDateTime] is a point on a time line in a particular calendar system, but with no concept of the offset from UTC.
In order to identify which specific instant in time a `LocalDateTime` refers to, you have to supply time zone or offset information.

LocalDate
---------

A [`LocalDate`][LocalDate] is simply the date portion of a `LocalDateTime` - it has no concept of the time of day; it's just a date.

LocalTime
---------

A [`LocalTime`][LocalTime] is simply the time portion of a `LocalDateTime` - it has no concept of the date on which the time occurs; it's just a time.

OffsetDateTime
--------------

An [`OffsetDateTime`][OffsetDateTime] is a `LocalDateTime` with an associated `Offset` - it uniquely identifies an `Instant`, but because the full time zone
information is missing, there's no indication what the local time would be 5 minutes later or earlier, as the offset within the time zone can change.
Dates and times are often transmitted between systems using this information - and the offset is often misnamed as a time zone or time zone identifier. It's not -
it's just an offset.

DateTimeZone
------------

A [`DateTimeZone`][DateTimeZone] (or just time zone) is a mapping between UTC
and local times. Many time zones alternate between two offsets over the course
of time, based on [daylight saving time][DST]. To obtain a time zone in Noda
Time, you have to choose where the information will come from, via an
`IDateTimeZoneProvider`. Two providers are built into Noda Time - one which
uses the tz database (also known as the IANA Time Zone database, or zoneinfo or
Olson database) and one to wrap the information provided by the BCL via
`TimeZoneInfo`. Both providers are available through
[`DateTimeZoneProviders`][DateTimeZoneProviders] as
`DateTimeZoneProviders.Tzdb` and `DateTimeZoneProviders.Bcl`. Once you've
chosen a provider, you can find the identifiers that provider publishes, and
fetch any specific time zone by ID.

ZonedDateTime
-------------

A [`ZonedDateTime`][ZonedDateTime] is a `LocalDateTime` within a specific time zone - with the added information of the exact `Offset`, in case of ambiguity. (During daylight
saving transitions, the same local date/time can occur twice.) An alternative way of looking at it is the combination of an `Instant`, a `DateTimeZone`, and a `CalendarSystem`.

Duration
--------

A [`Duration`][Duration] is simply a number of ticks, which can be added to (or subtracted from) an `Instant` or a `ZonedDateTime`. A particular value will always represent the same
amount of elapsed time, however it's used.

Period
------

A [`Period`][Period] is a number of years, months, weeks, days, hours and so on, which can be added to (or subtracted from) a `LocalDateTime`, `LocalDate` or `LocalTime`. The amount of
elapsed time represented by a `Period` isn't fixed: a period of "one month" is effectively longer when added to January 1st than when added to February 1st, because February is always shorter than
January.

IClock
------

An [`IClock`][IClock] implementation provides information about the current instant. It knows nothing about time zones or calendar systems. For [testability](testing.html), this is defined
as an interface; in a production deployment you're likely to use the [`SystemClock`][SystemClock] singleton implementation.

Interval
--------

An [`Interval`][Interval] is simply two instants - a start and an end. The interval includes the start, and excludes the end, which means that if you have abutting intervals any instant will be in
exactly one of those intervals.

[DST]: http://en.wikipedia.org/wiki/Daylight_saving_time
[Interval]: noda-type://NodaTime.Interval
[LocalTime]: noda-type://NodaTime.LocalTime
[LocalDate]: noda-type://NodaTime.LocalDate
[LocalDateTime]: noda-type://NodaTime.LocalDateTime
[Instant]: noda-type://NodaTime.Instant
[CalendarSystem]: noda-type://NodaTime.CalendarSystem
[DateTimeZone]: noda-type://NodaTime.DateTimeZone
[IDateTimeZoneProvider]: noda-type://NodaTime.IDateTimeZoneProvider
[DateTimeZoneProviders]: noda-type://NodaTime.DateTimeZoneProviders
[Offset]: noda-type://NodaTime.Offset
[Period]: noda-type://NodaTime.Period
[Duration]: noda-type://NodaTime.Duration
[OffsetDateTime]: noda-type://NodaTime.OffsetDateTime
[ZonedDateTime]: noda-type://NodaTime.ZonedDateTime
[IDateTimeZoneProvider]: noda-type://NodaTime.IDateTimeZoneProvider
[DateTimeZoneProviders]: noda-type://NodaTime.DateTimeZoneProviders
[IClock]: noda-type://NodaTime.IClock
[SystemClock]: noda-type://NodaTime.SystemClock
