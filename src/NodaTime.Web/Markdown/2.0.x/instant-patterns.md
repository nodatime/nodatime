@Title="Patterns for Instant values"

The [`Instant`](noda-type://NodaTime.Instant) type supports the following patterns:

Standard Patterns
-----------------

The following standard pattern is supported:

- `g`: General format pattern.
  The ISO-8601 representation of this instant in UTC, using the
  pattern "uuuu-MM-ddTHH:mm:ss'Z'" and always using the invariant culture,
  with the default "start of time" and "end of time" labels.
  This is the default format pattern.

Custom Patterns
---------------

[`Instant`](noda-type://NodaTime.Instant) supports all the [`LocalDateTime` custom patterns](localdatetime-patterns).
The pattern allows the culture to be specified, but *always* uses the ISO-8601 calendar, and *always* uses the UTC
time zone. The "template value" is always the Unix epoch.
