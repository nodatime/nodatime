@Title="Patterns for OffsetDateTime values"

The [`OffsetDateTime`](noda-type://NodaTime.OffsetDateTime) type supports the following patterns:

Standard Patterns
-----------------

- `G`: General invariant ISO-8601 pattern, down to the second. This corresponds to the custom pattern `uuuu'-'MM'-'dd'T'HH':'mm':'sso<G>`. This is the default format pattern.
- `o`: Extended invariant ISO-8601 pattern, down to the nanosecond. This will round-trip values except for the calendar system. This corresponds to the custom pattern `uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G>`.
- `r`: Full round-trip pattern including calendar system. This corresponds to the custom pattern `uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G> '('c')'`.

Custom Patterns
---------------

The custom format patterns for the local date and time parts of the value are the same as the [custom patterns for `LocalDateTime`](localdatetime-patterns). There is an additional specifier for the offset.

The "o" specifier must always be followed by a [pattern for `Offset`](offset-patterns) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of `uuuu-MM-dd HH:mm:ss o<G>` might produce output of "2013-07-17 06:20:35 Z" or "2013-07-17 07:20:35 +01:00".

To use culture-specific standard date or time patterns in a custom `OffsetDateTime` pattern, use some combination of the following specifiers:

- `ld<...>`: The `LocalDate` pattern within angle brackets
- `lt<...>`: The `LocalTime` pattern within angle brackets
- `ldt<...>`: The `LocalDateTime` pattern within angle brackets

For example, to use a culture-specific short date format, but a fixed time format,
followed by the offset in general form, you might use a pattern of `ld<d> HH:mm:ss o<G>`
