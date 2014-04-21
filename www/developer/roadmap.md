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

This roadmap was last updated on **2014-04-21**.

- 1.3
  - Use of ReSharper annotations to allow better code analysis, both of Noda Time code itself and code using Noda Time
  - Provide more information from CLDR for time zone information
    (e.g. sample cities to display to users)
  - Enable embedding Noda Time in SQLCLR
  - Support for the Persian (solar Hijri) calendar
  - Experimental support for the Hebrew calendar

- 1.4
  - Support month name parsing/formatting for the Hebrew calendar (support leap months)
  - Fix any issues with the previously-experimental Hebrew calendar calculations (e.g. what it means to add a year)
  - Use CLDR for other some resources (month/day names, and possibly date/time formats, via new pattern classes)
  - Possibly support Ethiopian and Um Al Qura calendars
  - Address the issues of values going outside the supported range.
  - Introduce new extension methods to the Testing assembly, and possibly make some existing methods in the main
    library into extension methods (e.g. in BclConversions).

Features currently scheduled for removal or breaking change in 2.0:

   - all obsolete API members
   - support for the legacy resource-based time zone data format
   - revisit all ordering and equality comparison operations, quite possibly
     rewriting them to be calendar-insensitive (and zone-insensitive in the case of ZonedDateTime)
