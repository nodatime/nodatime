---
layout: userguide
title: Patterns for OffsetDateTime values
category: text
weight: 2080
---

The [`OffsetDateTime`](noda-type://NodaTime.OffsetDateTime) type supports the following patterns:

Standard Patterns
-----------------

- `G`: General invariant ISO-8601 pattern, down to the second. This corresponds to the custom pattern `yyyy'-'MM'-'dd'T'HH':'mm':'sso<G>`.
- `o`: Extended invariant ISO-8601 pattern, down to the tick. This will round-trip values except for the calendar system. This corresponds to the custom pattern `yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G>`.
- `r`: Full round-trip pattern including calendar system. This corresponds to the custom pattern `yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G> '('c')'`.

Custom Patterns
---------------

The custom format patterns for local date and time are the same as the [custom patterns for `LocalDateTime`](localdatetime-patterns.html) with one extra specifier for the offset.

The "o" specifier must always be followed by a [pattern for `Offset`](offset-patterns.html) within angle brackets. The pattern may be a standard pattern or a custom pattern. For example, a pattern of "yyyy-MM-dd HH:mm:ss o&lt;G&gt;" might produce output of "2013-07-17 06:20:35 Z" or "2013-07-17 07:20:35 +01:00".
