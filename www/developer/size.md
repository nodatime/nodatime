---
layout: developer
title: Sizes of core types
weight: 600
---

Many of Noda Time's core types are value types, and their size needs to be carefully
managed when features are added or removed. The following gives an indication of what
each of the value types consists of, and any possible space savings in the future.

#Duration#

`Duration` consists of:

- `int days`
- `long nanoOfDay`

(12 bytes)

The `days` field may be negative, whereas `nanoOfDay` is always non-negative. (So to represent a duration of -1ns, you'd use a `days` value of -1 and a `nanoOfDay` value of `NodaConstants.NanosecondsPerStandardDay - 1`.) While this may seem odd for a duration type, it fits in well with the layout of other types, particularly `LocalDate` and `LocalTime`.

#Instant and LocalInstant#

`Instant` and `LocalInstant` each simply have a `Duration` field:

- `Duration duration`

(12 bytes)

#LocalTime#

A `LocalTime` only knows about the nanosecond of the day, which is represented as a `long`:

- `long nanoseconds`

(8 bytes)

The value is always non-negative, and requires 47 bits (to represent a maximum value one less than 86,400,000,000,000).

#YearMonthDay and YearMonthDayCalendar#

`YearMonthDay` and `YearMonthDayCalendar` are used to split a date into year, month and day. Its representation is just a single `int`:

- `int value`

(4 bytes)

The value is split into the three or four parts as:

- Day: 6 bits
- Month: 4 bits
- Year: 15 bits
- Calendar: 7 bits (only in `YearMonthDayCalendar`)

`YearMonthDay` is used within `YearMonthCalculator` code, whereas `YearMonthDayCalendar` is used in `LocalDate` and `OffsetDateTime`.
The calendar is represented in 7 bits by assigning an ordinal to each calendar system. This approach limits Noda Time to having
a maximum of 128 calendar systems (including variants such as leap year patterns, epochs and month numbering systems). However,
as of August 2014 it seems unlikely that we'll ever hit that limit. Perhaps more importantly, it does mean that we can't easily
allow user-provided calendar systems.

The ISO calendar system is calendar 0, making it a natural default for the type. The other components are encoded such that a 0
in the value means 1 in the component itself, so the default values of `YearMonthDay` and `YearMonthDayCalendar` are 0001-01-01
and 0001-01-01 ISO respectively.

#LocalDate#

A `LocalDate` is simply a `YearMonthDayCalendar`:

- `YearMonthDayCalendar yearMonthDayCalendar`

(4 bytes)

#LocalDateTime#

A `LocalDateTime` is simply the combination of a `LocalDate` and a `LocalTime`:

- `LocalDate date`
- `LocalTime time`

(12 bytes)

#Offset#

An `Offset` stores the number of seconds difference
between UTC and local time. This is within inclusive bounds of +/- 18 hours.

- `int seconds`

(4 bytes; 17 bits used)

#OffsetDateTime#

An `OffsetDateTime` is *logically* a `LocalDateTime` and an `Offset`, but it's stored somewhat differently,
as that has shown some surprising performance benefits:

- `YearMonthDayCalendar yearMonthDayCalendar`
- `long nanosecondsAndOffset`

(12 bytes)

The nanosecondsAndOffset value is split into two parts as:

- Nanosecond-of-day: 47 bits
- Offset: 17 bits

#ZonedDateTime#

A `ZonedDateTime` is an `OffsetDateTime` and a `DateTimeZone` reference:

- `OffsetDateTime offsetDateTime`
- `DateTimeZone zone`

(12 bytes + 1 reference)
