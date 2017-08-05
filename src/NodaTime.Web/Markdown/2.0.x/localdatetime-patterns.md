@Title="Patterns for LocalDateTime values"

The [`LocalDateTime`](noda-type://NodaTime.LocalDateTime) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `o` or `O`: The BCL round-trip pattern, which is always "uuuu'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff" using the
  invariant culture. The calendar system is not round-tripped in this pattern, but it's compatible with the
  BCL round-trip pattern (for `DateTime` values with a `Kind` of `Unspecified`, which is closest in meaning to
  `LocalDateTime`). Note that this only has 7 decimal digits for sub-second precision, so it can lose data
  for values which have a non-zero "nanosecond of tick". This lack of precision is maintained for compatibility
  with the BCL. Use `R` for the equivalent pattern with 9 digits of sub-second precision.

- `r`: The full round-trip pattern including calendar system, which is always "uuuu'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff '('c')'" using the invariant culture.

- `R`: The full round-trip pattern without calendar system, which is always "uuuu'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff" using the invariant culture.

- `s`: The sortable pattern, which is always "uuuu'-'MM'-'dd'T'HH':'mm':'ss" using the invariant culture. (Note: this is only truly sortable for years within the range \[0-9999\].)

- `f`: The culture's [long date pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longdatepattern.aspx) followed by a space,
  followed by the [short time pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shorttimepattern.aspx).

- `F`: The full date and time pattern as defined by the culture's [`DateTimeFormatInfo.FullDateTimePattern`](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.fulldatetimepattern.aspx)
  For example, in the invariant culture this is "dddd, dd MMMM yyyy HH:mm:ss".

- `g`: The culture's [short date pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shortdatepattern.aspx) followed by a space,
  followed by the [short time pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shorttimepattern.aspx).

- `G`: The culture's [short date pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shortdatepattern.aspx) followed by a space,
  followed by the [long time pattern](https://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longtimepattern.aspx).
  This is the default format pattern.

Custom Patterns
---------------

The custom format patterns for local date and time values are provided by combining the [custom patterns for `LocalDate`](localdate-patterns) with
the [custom patterns for `LocalTime`](localtime-patterns). The result is simply the combination of the date and the time.

There are two exceptions to this:

- When parsing a `LocalDateTime`, an 24-hour (`HH`) specifier is allowed to have the value 24, instead of being
  limited to the range 00-23. This is only permitted if the resulting time of day is midnight, and it indicates
  the end of the specified day. The result is midnight on the following day. For example, using the ISO pattern,
  the values `2012-11-24T24:00:00` and `2012-11-25T00:00:00` are equivalent. A value of 24 is never produced when
  formatting.
- The character 'T' is allowed to be unquoted, and acts as a single-character literal. This is to simplify the very
  common case where 'T' is used to separate the date and time parts of a value.

Additionally, a specifier of `lt<...>` is used to embed a `LocalTime` pattern (which may be a standard pattern)
in a custom pattern, and likewise a specifier of `ld<...>` is used to embed a `LocalDate` pattern. For example, a custom
pattern of `'Date: ' ld<d>'; Time: ' lt<T>` will use the standard short format date pattern, and the standard long format
time pattern for the appropriate culture for the date and time parts.
