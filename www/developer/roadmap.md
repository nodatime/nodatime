---
layout: developer
title: Current roadmap
---

The following is an _approximate_ roadmap for the major features that
we hope to support in Noda Time.  Some of this roadmap is inspired by
Noda Time's [current limitations][].

[current limitations]: http://nodatime.org/userguide/limitations.html

If there's something not mentioned here that you feel should be on this
roadmap, then *please* either raise an issue or post on the
[mailing list](http://groups.google.com/group/noda-time).

This roadmap was last updated on **2014-06-16**.

- 1.3
  - Use of ReSharper annotations to allow better code analysis, both of Noda Time code itself and code using Noda Time
  - Support for the Persian (solar Hijri) calendar
  - Experimental support for the Hebrew calendar

- 2.0
  - Remove all obsolete API members
  - Remove support for the legacy resource-based time zone data format
  - Make all public static fields properties
  - Revisit all ordering and equality comparison operations. (Many may be removed, others may
    be exposed via specific comparers.)
  - Change granularity to be nanoseconds
  - Change internal representations (almost entirely hidden from the API, but will have
    performance impact - probably in both directions)
  - Introduce "adjusters" - functions such as "end of month", "next Wednesday",
    "truncate to second" which can be expressed in a unified API. Similar specific methods
    in the API may be removed.
  - Fix handling of all extreme values, with well-understood limits, no "beginning of time"
    values, etc.
  - Introduce new extension methods in Testing assembly, and make some existing methods
    extension methods. (We're pretty sure we don't need to support .NET 2.0, ever...)
  - Rename some methods which map cleanly to those in the BCL.
  - Consider adding a time zone to a clock, making it easier to express "now in the local time 
    zone" in one go.

- Unscheduled features and issues which we hope to address at some point:
  - Support month name parsing/formatting for the Hebrew calendar using `CultureInfo`
  - Ethopian and Um Al Qura calendars
  - Enable embedding Noda Time in SQLCLR
  - More code analysis using Roslyn (e.g. check that all non-nullable parameters are validated)

- Separate CLDR project
  - Formatting (probably not parsing) using CLDR data for month names, Unicode pattern symbols, possibly non-ASCII numbers.
  - CLDR time zone information (e.g. sample cities to display to users)
