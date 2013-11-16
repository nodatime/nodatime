---
layout: userguide
title: Patterns for Period values
category: text
weight: 2100
---

Currently, the [`Period`](noda-type://NodaTime.Period) type doesn't support custom patterns, but two predefined patterns
which are exposed in [`PeriodPattern`](noda-type://NodaTime.Text.PeriodPattern):

Roundtrip (`PeriodPattern.RoundtripPattern`)
--------------------------------------------

This pattern performs no normalization of the period - it simply writes out the value of each component of the period
(as determined by its units). The format aims to be at least *reminiscent* of ISO-8601, and in many cases it will be compatible
with ISO-8601, but it shouldn't be used in places where ISO compliance is required.

The format is:

    P (date components) T (time components)

Where each non-zero component within the period is specified as its value followed by a unit specifier from this list:

- `Y` (years)
- `M` (months in the date portion)
- `W` (weeks)
- `D` (days)
- `H` (hours)
- `M` (minutes in the date portion)
- `S` (seconds)
- `s` (milliseconds)
- `t` (ticks)

The `T` is omitted if there are no time components.

This format differs from ISO-8601 in the following ways:

- Values are *always* integers; the ISO value of `PT10.5S` is invalid in this pattern, for example
- Values can be negative
- The `s` and `t` units aren't part of ISO

Normalized ISO-like (`PeriodPattern.NormalizedIsoPattern`)
----------------------------------------------------------

This pattern normalizes the week, day and time parts of the period before formatting (even if the period
only contains weeks, currently), assuming that a week is always 7 days and a day is always 24 hours. The result of the normalization
removes weeks entirely, so a period of 2 weeks becomes 14 days. The month and year components are left alone, as they may vary in duration.
The result of the normalization is then serialized in a way which is *mostly* compatible with ISO-8601.

The general format is as for the roundtrip pattern, but without the `s` and `t` units; weeks are currently never formatted
due to normalization.

Only non-zero components are formatted. The exception to this is the zero
period, which is formatted as "P0D".

Any fractional seconds are formatted using a period as a decimal separator, with a leading 0 if necessary. So
for example, a 500 millisecond period is formatted as `PT0.5S`.

While this pattern is more ISO-like than the roundtrip pattern, it can still *produce* a text representation which is not valid in ISO,
as it values may be negative. Any valid ISO representation can be *parsed* correctly, however.

Methods on `Period`
-------------------

At the time of this writing, there are no methods performing parsing within `Period`, and only the parameterless `ToString` method is
supported, which always uses the roundtrip pattern described above. If patterns based on text representations are ever supported, these
methods will be implemented on `Period` in the normal way.
