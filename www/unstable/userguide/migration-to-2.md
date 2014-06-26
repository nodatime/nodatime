---
layout: userguide
title: Migrating from 1.x to 2.0
category: core
weight: 1050
---

Noda Time 2.0 contains a number of breaking changes. If you have a project which uses Noda Time
1.x and are considering upgrading to 2.0, please read the following migration guide carefully.
In particular, there are some changes which are changes to execution-time behaviour, and won't show up as compile-time errors.

Obsolete members
====

A few members in 1.x were already marked as obsolete, and they have now been removed. Code using 
these members will no longer compile. Two of these were simple typos in the name - fixing code 
using these is simply a matter of using the correct name instead:

- `Era.AnnoMartyrm` should be `Era.AnnoMartyrum`
- `Period.FromMillseconds` should be `Period.FromMilliseconds`

In addition, `DateTimeZoneProviders.Default` has been removed. It wasn't the default in any Noda 
Time code, and it's clearer to use the `DateTimeZoneProviders.Tzdb` member, which the `Default`
member was equivalent to anyway.

Support for the resource-based time zone database format was removed in Noda Time 2.0. In terms
of the public API, this just meant removing three `TzdbDateTimeZoneSource` constructors, and
removing some documented edge cases where the legacy resource format didn't include as much
information as the more recent "nzd" format. If you were previously using the resource format,
just move to the "nzd" format, using the static factory members of `TzdbDateTimeZoneSource`.

Removed (or now private) members
====

The `Instant(long)` constructor is now private; use `Instant.FromTicksSinceUnixEpoch` instead.
As the resolution of 2.0 is nanoseconds, a constructor taking a number of *ticks* since the
Unix epoch is confusing. The static method is self-describing, and this allows the constructor
to be rewritten for use within Noda Time itself.

The `LocalDate.LocalDateTime` property has been removed. It was rarely a good idea to
arbitrarily pick the Unix epoch as the date, and usually indicates a broken design. If you
still need this behaviour, you can easily construct a `LocalDate` for the Unix epoch and use
the addition operator instead.

`CalendarSystem.GetMaxMonth` has been renamed to `GetMonthsInYear`, to match the equivalent
method in `System.Globalization.Calendar`.

Period
====

The `Years`, `Months`, `Weeks` and `Days` properties of `Period` (and `PeriodBuilder`) are
now `int` rather than `long` properties. Any property for those units outside the range of `int` 
could never be added to a date anyway, as it would immediately go out of range. This change just
makes that clearer, and embraces the new "`int` for dates, `long` for times" approach which 
applies throughout Noda Time 2.0. The indexer for `PeriodBuilder` is still of type `long`, but 
it will throw an `ArgumentOutOfRangeException` for values outside the range of `int` when 
setting date-based units.

Normalization of a period which has time units which add up to a "days" range outside the range
of `int` will similarly fail.

Serialization
====

TBD (this will be awkward). To note so far:

- Periods with year/month/week/day values outside the range of `int`

Default values
====

The default values of some structs have changed, from returning the Unix epoch to returning January 1st 1AD (at midnight where applicable):

- `LocalDate`
- `LocalDateTime`
- `ZonedDateTime` (in UTC, as in 1.x)
- `OffsetDateTime` (with an offset of 0, as in 1.x)

(The fate of `Instant` remains unknown at this point...)

We recommend that you avoid relying on the default values at all - partly for the sake of clarity.