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
