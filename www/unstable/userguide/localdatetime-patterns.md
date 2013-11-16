---
layout: userguide
title: Patterns for LocalDateTime values
category: text
weight: 2070
---

The [`LocalDateTime`](noda-type://NodaTime.LocalDateTime) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `o` or `O`: The round-trip pattern, which is always "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff" using the invariant culture. The calendar
  system is not round-tripped in this pattern, but it's compatible with the BCL round-trip pattern (for `DateTime` values with a `Kind` of `Unspecified`, which is closest in meaning to `LocalDateTime`).

- `r`: The full round-trip pattern including calendar system, which is always "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff '('c')'" using the invariant culture.

- `s`: The sortable pattern, which is always "yyyy'-'MM'-'dd'T'HH':'mm':'ss" using the invariant culture. (Note: this is only truly sortable for years within the range [0-9999].)

- `f`: The culture's [long date pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longdatepattern.aspx) followed by a space,
  followed by the [short time pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shorttimepattern.aspx).

- `F`: The full date and time pattern as defined by the culture's [`DateTimeFormatInfo.FullDateTimePattern`](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.fulldatetimepattern.aspx) 
  For example, in the invariant culture this is "dddd, dd MMMM yyyy HH:mm:ss".

- `g`: The culture's [short date pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shortdatepattern.aspx) followed by a space,
  followed by the [short time pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shorttimepattern.aspx).

- `G`: The culture's [short date pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shortdatepattern.aspx) followed by a space,
  followed by the [long time pattern](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longtimepattern.aspx).

Custom Patterns
---------------

The custom format patterns for local date and time values are provided by combining the [custom patterns for `LocalDate`](localdate-patterns.html) with
the [custom patterns for `LocalTime`](localtime-patterns.html). The result is simply the combination of the date and the time.

There is one exception to this: when parsing a `LocalDateTime`, an 24-hour (`HH`) specifier is allowed to have the value 24,
instead of being limited to the range 00-23. This is only permitted if the resulting time of day is midnight, and it indicates
the end of the specified day. The result is midnight on the following day. For example, using the ISO pattern, the values
`2012-11-24T24:00:00` and `2012-11-25T00:00:00` are equivalent. A value of 24 is never produced when formatting.

