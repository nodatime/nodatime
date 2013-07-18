---
layout: userguide
title: Patterns for ZonedDateTime values
category: text
weight: 95
---

The [`ZonedDateTime`](noda-type://NodaTime.ZonedDateTime) type supports the following patterns:

Standard Patterns
-----------------

TBD.

Custom Patterns
---------------

The custom format patterns for a zoned date and time are provided by combining the [custom patterns for `OffsetDateTime`](offsetdatetime-patterns.html) with
the addition of two extra custom format specifiers: 'z' and 'x'.

'z' is used to parse or format that time zone identifier. When parsing, an ]`IDateTimeZoneProvider`](noda-type://NodaTime.IDateTimeZoneProvider) is used to extract candidate identifiers and fetch time zones for them. The UTC+/-xx:xx format for fixed offset time zones is always valid, regardless of provider. The provider is part of the `ZonedDateTimePattern`, and a new pattern with a different provider can be created using the `WithZoneProvider` method. The provider is not used when formatting: the time zone identifier is simply used directly. Note that if you format a `ZonedDateTime` which uses a time zone from a different provider than the one in the pattern, you may not be able to parse it again with the same pattern.

`x` is used *only* for formatting; it includes the abbreviation associated with the time zone at the given time, such as "PST" or "PDT". This is format-only as abbreviations are often ambiguous; they are not a substitute for full time zone identifiers.

When parsing, if the pattern does not contain the `z` specifier, the time zone from the default value is used. The standard patterns all use a default value with the UTC time zone.

If the pattern does not contain an offset specifier ("o&lt;...&gt;") the local date and time represented by the text is interpreted according to the [`ZoneLocalMappingResolver`](noda-type://NodaTime.TimeZones.ZoneLocalMappingResolver) associated with the pattern. A new pattern can be created from an existing one, just with a different resolver, using the `WithResolver` method. If the resolver throws a `SkippedTimeException` or `AmbiguousTimeException`, these are converted into `UnparsableValueException` results. Note that a pattern without an offset specifier will always lead to potential data loss when used with time zones which aren't a single fixed offset, due to the normal issues of time zone transitions (typically for daylight saving time). 

If the pattern *does* contain an offset specifier, then when parsing, the offset present in the text is validated against the time zone. By specifying both a time zone identifier and an offset, the ambiguity around time zone transitions is eliminated. Again, if the offset is invalid for the time zone at the given local date and time, an `UnparsableValueException` result is produced.

