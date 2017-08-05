@Title="Patterns for Instant values"

The [`Instant`](noda-type://NodaTime.Instant) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `g`: General format pattern.
  The ISO-8601 representation of this instant in UTC, using the
  pattern "yyyy-MM-ddTHH:mm:ss'Z'" and always using the invariant culture,
  with the default "start of time" and "end of time" labels.
  This is the default format pattern.

- `n`: Numeric with thousand separators.
  This gives the number of ticks since the Unix epoch as an integer,
  including thousands separators. Sample on September 16th 2011:
  "13,161,548,674,473,131"

- `d`: Numeric without thousand separators.
  This gives the number of ticks since the Unix epoch as an integer,
  not including thousands separators. Sample on September 16th 2011:
  "13161548674473131"

Custom Patterns
---------------

[`Instant`](noda-type://NodaTime.Instant) supports all the [`LocalDateTime` custom patterns](localdatetime-patterns).
The pattern allows the culture to be specified, but *always* uses the ISO-8601 calendar, and *always* uses the UTC
time zone. The "template value" is always the Unix epoch.

All instant patterns (other than the standard *numeric* ones) handle [`Instant.MinValue`](noda-property://NodaTime.Instant.MinValue)
and [`Instant.MaxValue`](noda-property://NodaTime.Instant.MaxValue) separately. The default formatting of the values
are simply "MinInstant" and "MaxInstant" respectively, but a new
[`InstantPattern`](noda-type://NodaTime.Text.InstantPattern) with different min/max labels
can be created using the
[`WithMinMaxLabels`](noda-type://NodaTime.Text.InstantPattern#NodaTime_Text_InstantPattern_WithMinMaxLabels_System_String_System_String_) method.
The labels must be non-empty strings which differ from each other.
