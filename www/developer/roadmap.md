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

This roadmap was last updated on **2013-11-13**.

- 1.3
  - Use of ReSharper annotations to allow better code analysis, both of Noda Time code itself and code using Noda Time
  - Provide more information from CLDR for time zone information
    (e.g. sample cities to display to users)
  - Use CLDR for other l10n resources
  - Enable embedding Noda Time in SQLCLR
  - Implement more calendar systems (candidates include the Persian, Hebrew, Ethiopian and Um Al Qura calendars)

- 1.4
  - Address the issues of values going outside the supported range. (Work may well start on this in a branch during 1.3 timeline, but probably won't gate the release. There's a lot to think about, and potentially significant performance impact.)

Features currently scheduled for removal or breaking change in 2.0:

   - all obsolete API members
   - support for the legacy resource-based time zone data format
   - revisit all ordering and equality comparison operations, quite possibly
     rewriting them to be calendar-insensitive
