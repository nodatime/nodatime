---
layout: userguide
title: Current roadmap
category: library
weight: 160
---

The following is an _approximate_ roadmap for the major features that
we hope to support in Noda Time.  Some of this roadmap is inspired by
Noda Time's [current limitations](limitations.html).

If there's something not mentioned here that you feel should be on this
roadmap, then *please* either raise an issue or post on the
[mailing list](http://groups.google.com/group/noda-time).

This roadmap was last updated on **2013-03-04**.

   * 1.2
      - Better serialization: add native support for XML serialization,
        complete the Json.NET serialization (and release as a package)
      - Better parsing/formatting: custom parsing and formatting for
        `ZonedDateTime` and `OffsetDateTime`; _possibly_ likewise for
        `Period`, and more consistency around access to standard
        patterns
      - Revisit `NodaFormatInfo`
      - Refactor calendar engine
   * 1.3
      - Provide more information from CLDR for time zone information
        (e.g. sample cities to display to users)
      - Possibly, use CLDR for other l10n resources

Features currently scheduled for removal or breaking change in 2.0:

   - all obsolete API members
   - support for the legacy resource-based time zone data format
   - revisit all ordering and equality comparison operations, quite possibly
     rewriting them to be calendar-insensitive