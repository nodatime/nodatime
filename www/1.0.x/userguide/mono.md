---
layout: userguide
title: Mono support
category: library
weight: 140
---

[Mono](http://mono-project.com) is an open source implementation of
the Common Language Infrastructure which runs on various platforms,
including Windows, Linux and OS X.

Noda Time runs on Mono, but with some limitations:

- Noda Time is not *developed* on Mono, so while releases will be tested
  against it (running all the unit tests), code which isn't part
  of a release may not work. Please raise an issue on the
  [tracker page](http://code.google.com/p/noda-time/issues/list) if
  you come across a breakage like this, and we'll fix it as soon
  as possible.
- `TimeZoneInfo` in Mono has some critical flaws in the latest stable
  Mono release at the time of this writing (2.10.8) - while
  [`BclDateTimeZone`](noda-type://NodaTime.TimeZones.BclDateTimeZone) *may*
  do the right thing, it may disagree with the results of calling
  methods directly on the time zone ([issue 97][]).
- Some cultures in Mono have standard date/time patterns including
  "z" for "offset from UTC". These will not display appropriately
  when used for text formatting in Noda Time, as the "z" is
  meaningless for local dates and times ([issue 98][]).
- Some cultures in Mono have standard date/time patterns which
  use the abbreviated am/pm designator, but have am/pm designators
  which are the same when abbreviated. In these cases, parsing
  is ambiguous.

The prebuilt binary packages can be used on Mono directly. To build from source,
See the ["Building and testing"][building] section in the developer guide.

[building]: http://nodatime.org/developer/building.html
