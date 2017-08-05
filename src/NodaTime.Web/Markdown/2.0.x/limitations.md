@Title="Limitations of Noda Time"

Noda Time is a work in progress. It has various limitations, some of
which we'd obviously like to remove over time. Here's a list of some
aspects we'd like to improve; see the
[issues list](https://github.com/nodatime/nodatime/issues) for
others.

We also have a [roadmap](roadmap) of intended releases. This is
always tentative, of course, but it helps to give some clarity to our
decisions in terms of what to work on next.

If there's something that should be within Noda Time's
scope, but we don't support it yet, then *please* either raise an
issue or post on the
[mailing list](https://groups.google.com/group/noda-time).

.NET Standard vs Desktop difference
===================================

The API in .NET Standard libraries is more limited than the
full desktop version. Currently this provides relatively few challenges for
Noda Time, with one significant exception: `TimeZoneInfo`. We can't fetch
arbitrary time zones by ID, nor can we ask for the adjustment rules for a particular
time zone.

The upshot of this is that we can't currently support
[`BclDateTimeZone`](noda-type://NodaTime.TimeZones.BclDateTimeZone)
on the `netstandard1.3` version of Noda Time.

Fuller text support
===================

While it is now possible (as of 1.2.0) to parse and format
[`ZonedDateTime`](noda-type://NodaTime.ZonedDateTime) and
[`OffsetDateTime`](noda-type://NodaTime.OffsetDateTime), our text support is
still lacking in some areas: some other types lack flexible formatting, and
we may want to optimize further at some point too.

Additionally, all our text localization resources (day and month names) come from the .NET
framework itself. That has some significant limitations, and makes Noda Time more reliant
on `CultureInfo` than is ideal. [CLDR](http://cldr.unicode.org/) contains more information,
which should allow for features such as ordinal day numbers ("1st", "2nd", "3rd") and
a broader set of supported calendar/culture combinations (such as English names for the
Hebrew calendar months).

Speaking of the Hebrew calendar, initial support for the calendar has been introduced
in 1.3.0, but month names are *not* properly supported currently. See [local date formatting](localdate-patterns) for more details of this limitation.

Better resource handling
========================

We'd like to be able to bundle appropriate patterns (and other
internationalizable materials) within Noda Time while keeping it as
a single DLL. (Satellite DLLs are fine for some scenarios, but messy
in others.) Additionally we'd like to allow these resources to be
augmented or replaced by the caller at execution time, to allow
hot-fixes for cultures which we don't support as well as we might.

More time zone information
==========================

[CLDR](http://cldr.unicode.org/) provides useful information about
time zones such as a canonical ID and user-friendly representations
(countries and sample cities). We'd also like to make it clearer
when one zoneinfo time zone is an alias for another.

More calendars
==============

There will probably always be more calendars we could support. The
highest priority is probably an adapter for the BCL calendars.

Smarter arithmetic
==================

As noted in the [arithmetic guide](arithmetic), arithmetic using
[`Period`](noda-type://NodaTime.Period) is pretty simplistic. We may
want something smarter, probably to go alongside the "dumb but
predictable" existing logic. This will definitely be driven by real
user requirements though - it would be far too easy to speculate.
