@Title="Current roadmap"

The following is an _approximate_ roadmap for the major features that
we hope to support in Noda Time.  Some of this roadmap is inspired by
Noda Time's [current limitations][].

[current limitations]: /userguide/limitations

If there's something not mentioned here that you feel should be on this
roadmap, then *please* either raise an issue or post on the
[mailing list](https://groups.google.com/group/noda-time).

This roadmap was last updated on **2017-03-31**.

**1.4**

Migration-enabling release, deprecated members in 1.3 that are not in 2.0, and introducing the
2.0-compatible equivalents.

**2.1**

- Deconstruction methods to work neatly with C# 7
- Possibly more calendars (Fiscal, Wondrous, 360-day)
- More convenience properties

**Unscheduled features and issues which we hope to address at some point**

- Support month name parsing/formatting for the Hebrew calendar using `CultureInfo`
- Ethiopian calendars
- More code analysis using Roslyn (e.g. check that all non-nullable parameters are validated)

**Separate CLDR project**

- Formatting (probably not parsing) using CLDR data for month names, Unicode pattern symbols, possibly non-ASCII numbers.
- CLDR time zone information (e.g. sample cities to display to users)
