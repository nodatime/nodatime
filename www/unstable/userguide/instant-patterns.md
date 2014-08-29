---
layout: userguide
title: Patterns for Instant values
category: text
weight: 2040
---

The [`Instant`](noda-type://NodaTime.Instant) type supports the following patterns:

Standard Patterns
-----------------

The following standard pattern is supported:

- `g`: General format pattern.  
  The ISO-8601 representation of this instant in UTC, using the
  pattern "yyyy-MM-ddTHH:mm:ss'Z'" and always using the invariant culture,
  with the default "start of time" and "end of time" labels.
  This is the default format pattern.

Custom Patterns
---------------

[`Instant`](noda-type://NodaTime.Instant) supports all the [`LocalDateTime` custom patterns](localdatetime-patterns.html).
The pattern allows the culture to be specified, but *always* uses the ISO-8601 calendar, and *always* uses the UTC
time zone. The "template value" is always the unix epoch.

All instant patterns handle [`Instant.MinValue`](noda-field://NodaTime.Instant.MinValue) and [`Instant.MaxValue`](noda-field://NodaTime.Instant.MaxValue) separately. The default formatting of the values are simply "MinInstant" and "MaxInstant" respectively, but a new [`InstantPattern`](noda-type://NodaTime.Text.InstantPattern) with different min/max labels can be created using the [`WithMinMaxLabels`](noda-method://NodaTime.Text.InstantPattern.WithMinMaxLabels) method. The labels must be non-empty strings which differ from each other.
