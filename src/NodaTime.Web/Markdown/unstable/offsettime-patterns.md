@Title="Patterns for OffsetTime values"

The [`OffsetTime`](noda-type://NodaTime.OffsetTime) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `G`: General invariant ISO-8601 pattern, to second precision. This corresponds to the custom pattern `HH':'mm':'sso<G>`. This is the default format pattern.
- `o`: Extended invariant ISO-8601 pattern, to nanosecond precision. This corresponds to the custom pattern `HH':'mm':'ss;FFFFFFFFFo<G>`.

Custom Patterns
---------------

The custom format patterns for the local time part of the value are the same as the [custom patterns for `LocalTime`](localtime-patterns).

There is an additional specifier for the offset.
The "o" specifier must always be followed by a [pattern for `Offset`](offset-patterns) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of `HH:mm o<G>` might produce output of "16:23 Z" or "16:23 +01:00".

To use culture-specific standard time patterns in a custom `OffsetTime` pattern, use the "l" specifier, followed by a [pattern for `LocalTime`](localtime-patterns) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of `l<T> o<+HH:mm>` might produce output of "16:23:52 +01:00".
