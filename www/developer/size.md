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

#YearMonthDay#

`YearMonthDay` is used to split a date into year, month and day in a particular calendar, which
isn't represented in the value itself. Its representation is just a single `int`:

- `int value`

(4 bytes)

The value is split into the three parts as:

- Day: 6 bits
- Month: 6 bits
- Year: 15 bits

This leaves 5 bits available for future expansion, should they be useful - while actually allowing 
for days and months of up to 64. (If we reasonably assume no day-of-month above 32 and no month 
above 16, we can save an extra 3 bits.) Each component stores the desired value minus one, i.e. a 
raw value of 0 represents 0001-01-01. This ensures that a "default" `YearMonthDay` (for example
in the context of `new LocalDate()`) represents a valid value.

With an extra 3 bits, we could *potentially* encode a calendar system ID into `YearMonthDay` as well,
so that `LocalDate` was just a wrapper around `YearMonthDay`. This would have an impact on our
ability to create arbitrary calendar systems later, however.

#LocalDate#

A `LocalDate` is a `YearMonthDay` and a calendar system reference:

- `YearMonthDay yearMonthDay`
- `CalendarSystem calendar`

(4 bytes + 1 reference)

#LocalDateTime#

A `LocalDateTime` is simply the combination of a `LocalDate` and a `LocalTime`:

- `LocalDate date`
- `LocalTime time`

(12 bytes + 1 reference)

#Offset#

An `Offset` stores the number of milliseconds (currently; this may change to seconds) difference
between UTC and local time. This is within exclusive bounds of +/- 1 day.

- `int milliseconds`

Note that even a representation in seconds still takes up 18 bits, as the range of 2 days is 172,800
seconds. If we limited ourselves to +/- 18 hours, we could use just 17 bits... which would allow
a `LocalTime` and an `Offset` to be stored in a single 64-bit integer.

#OffsetDateTime#

An `OffsetDateTime` is *logically* a `LocalDateTime` and an `Offset`, but it's stored with separate
fields, as that has shown some surprising performance benefits:

- `YearMonthDay yearMonthDay`
- `LocalTime time`
- `Offset offset`
- `CalendarSystem calendar`

(16 bytes + 1 reference - theoretically. In reality, it seems that the 64-bit CLR (as of 2014-07-23)
treats the `Offset` and `YearMonthDay` values as 8 bytes each, rather than packing them
efficiently. That leads to a total size of 32 bytes.)

#ZonedDateTime#

A `ZonedDateTime` is an `OffsetDateTime` and a `DateTimeZone` reference:

- `OffsetDateTime offsetDateTime`
- `DateTimeZone zone`

(In theory, 16 bytes + 2 references; in reality, 40 bytes at the moment)
