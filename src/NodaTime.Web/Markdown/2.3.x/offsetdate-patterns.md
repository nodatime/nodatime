@Title="Patterns for OffsetDate values"

The [`OffsetDate`](noda-type://NodaTime.OffsetDate) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `G`: General invariant ISO-8601 pattern. This corresponds to the custom pattern `uuuu'-'MM'-'ddo<G>`. This is the default format pattern.
- `r`: Full round-trip pattern including calendar system. This corresponds to the custom pattern `uuuu'-'MM'-'ddo<G> '('c')'`.

Custom Patterns
---------------

The custom format patterns for the local date part of the value are the same as the [custom patterns for `LocalDate`](localdate-patterns).

There is an additional specifier for the offset.
The "o" specifier must always be followed by a [pattern for `Offset`](offset-patterns) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of `uuuu-MM-dd o<G>` might produce output of "2013-07-17 Z" or "2013-07-17 +01:00".

To use culture-specific standard date patterns in a custom `OffsetDate` pattern, use the "l" specifier, followed by a [pattern for `LocalDate`](localdate-patterns) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of `l<D> o<+HH:mm>` might produce output of "07/17/2013 +01:00".
