@Title="Current roadmap"

The following is an _approximate_ roadmap for the major features that
we hope to support in Noda Time.  Some of this roadmap is inspired by
Noda Time's [current limitations][].

[current limitations]: /userguide/limitations

If there's something not mentioned here that you feel should be on this
roadmap, then *please* either raise an issue or post on the
[mailing list](https://groups.google.com/group/noda-time).

This roadmap was last updated on **2018-05-06**.

**3.0**

- Removal of any obsolete methods
- Move to target netstandard2.0 only, removing all conditional code (if possible)
- Complete removal of binary serialization
- Make all structs readonly and try using `in` parameters in internal API (not public)
- Add parsing of `ReadOnlySpan<char>`
- Express nullability via C# 8
- Implement C# 8 range functionality (if that happens)

**Unscheduled features and issues which we hope to address at some point**

- Support month name parsing/formatting for the Hebrew calendar using `CultureInfo`
- More code analysis using Roslyn (e.g. check that all non-nullable parameters are validated)
- Possibly more calendars (Ethiopian, Fiscal, 360-day)

**Tooling and web site**

- The snippets infrastructure is mostly hacked together, largely
  written late at night at a conference. Ideally, it should be
  outside Noda Time entirely - but whatever we do, more work is
  needed. Lots more snippets need to be written, too :)
- Better benchmark infrastructure:
  - Nice UI with visualizations
  - More appropriate storage of benchmarks (in a database, basically)
- Revisit how we manage API documentation versions. This is
  particularly important in terms of serialization, which is now
  separated from the main repo.
- Web site revamp: it's functional, but could be more inviting.

**Separate projects**

- Formatting (probably not parsing) using CLDR data for month names, Unicode pattern symbols, possibly non-ASCII numbers.
- CLDR time zone information (e.g. sample cities to display to users)
- Serialization for Google Protocol Buffers ([code already started](https://github.com/nodatime/nodatime.serialization/tree/master/src/NodaTime.Serialization.Protobuf))