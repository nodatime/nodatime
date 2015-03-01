---
layout: userguide
title: Range of valid values
category: advanced
weight: 3015
---

(None of this is implemented yet!)

Noda Time can only represent values within a limited range. This is for pragmatic reasons:
while it would be possible to try to model every instant since some estimate of the big
bang until some expected "end of time," this would come at a significant cost to the
experience of the 99.99% of programmers who just need to deal with dates and times in a
reasonably modern (but not far-flung future) era.

The Noda Time 1.x releases supported years between around -27000 and +32000 (in the Gregorian
calendar), but had significant issues around the extremes. Noda Time 2.0 has a smaller range,
with more clearly-defined behaviour.

Ranges for calendars
-----

The "main" calendar for Noda Time is the Gregorian calendar. (We actually use `CalendarSystem.Iso`
by default, but that's just the Gregorian calendar with slightly different numbering around centuries;
it doesn't affect ranges.) This is the maximal calendar: a date in any other calendar can *always* be
converted to the Gregorian calendar.

Additionally, all calendars are restricted to four digit formats, even in year-of-era representations,
which avoids ever having to parse 5-digit years. This leads to a Gregorian calendar from 9999BC to
9999AD inclusive, or -9998 to 9999 in "absolute" years. The range of other calendars is determined from this
and from natural restrictions (such as not being proleptic).

The date range is always a complete number of years - the range shown below is inclusive at both ends, so every
date from the start of the minimum year to the end of the maximum year is valid. (The min/max Gregorian year
shows the Gregorian year corresponding to the min/max calendar values; this does *not* mean that the whole of
that Gregorian year can be converted into the relevant calendar.)

<table>
	<thead>
		<tr>
			<th>Calendar</th>
			<th>Min year</th>
			<th>Max year</th>
			<th>Min year (Gregorian)</th>
			<th>Max year (Gregorian)</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>Gregorian/ISO</td>
			<td>-9998</td>
			<td>9999</td>
			<td>-9998</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Julian</td>
			<td>-9997</td>
			<td>9998</td>
			<td>-9998</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Islamic</td>
			<td>1</td>
			<td>9665</td>
			<td>622</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Persian</td>
			<td>1</td>
			<td>9377</td>
			<td>622</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Hebrew</td>
			<td>1</td>
			<td>9999</td>
			<td>-3760</td>
			<td>6239</td>
		</tr>
		<tr>
			<td>Coptic</td>
			<td>1</td>
			<td>9715</td>
			<td>284</td>
			<td>9999</td>
		</tr>		
	</tbody>
</table>

Instant
----

The range for [`Instant`](noda-type://NodaTime.Instant) is simply the range of the Gregorian calendar, in UTC.
In other words, it covers -9998-01-01T00:00:00Z to 9999-12-031T23:59:59.999999999Z inclusive.

`LocalDateTime`, `LocalDate`, `ZonedDateTime`, and `OffsetDateTime`
----

All of the types in this heading are based on dates in calendars, and they all have ranges based on that
*local* date (and thus on the calendar). So even if the logical position on the global time-line for a
`ZonedDateTime` or `OffsetDateTime` is out of the range of `Instant`, the value is still valid. For example, consider
this code:

    LocalDate earliestDate = new LocalDate(-9998, 1, 1);
    OffsetDateTime offsetDateTime = earliestDate.AtMidnight().WithOffset(Offset.FromHours(10));

Here, `offsetDateTime` has a local date/time value of -9998-01-01T00:00:00, but it's 10 hours ahead of UTC... giving a logical
instant of -9999-12-31T14:00:00Z... which is out of range. That's fine, and the code above won't throw an exception... unless
you try to convert `offsetDateTime` to an `Instant`.

`Duration`
----

[`Duration`](noda-type://NodaTime.Duration) is designed to allow for *at least* the largest difference in
valid `Instant` values in either direction. As such, it needs to cover 631,075,881,599,999,999,999 nanoseconds -
which is just shy of 7,304,119 days. Internally, durations are stored in terms of "day" and "nanosecond within the day" (an 
implementation detail to be sure, but one which sometimes affects other decisions).

Additionally, it seems useful to be able to cover the full range of
[`TimeSpan`](http://msdn.microsoft.com/en-us/library/system.timespan), given that `Duration` is meant to be the roughly-equivalent
type.

The result is that we have a range of days from -2<sup>24</sup> to +2<sup>24</sup>-1 - and the nanosecond part means that the
total range is from -2<sup>24</sup> days inclusive to +2<sup>24</sup> days exclusive - the largest valid `Duration` is 1 
nanosecond less than 2<sup>24</sup> days.

`Period`
----

TBD. (Depends on the redesign...)

Failure modes
----

Assuming Noda Time is bug free<sup>1</sup>, we ensure that if you try to create a value outside the valid
range for that type (via any normal API calls), an exception of one of the following types will be thrown:

- `OverflowException`
- `ArgumentOutOfRangeException`
- `ArgumentException`
- `InvalidOperationException`

(TBD: Serialization? We should probably protect against invalid binary data.)

The "best" exception in each case depends very much on the context - but in the aim of code reuse and
performance, we may not always have that context available when the exception is thrown. For example, a call
to `OffsetDateTime.ToInstant()` doesn't have any arguments - but internally we may end up calling another method
and passing one or more arguments. If the validation of the "inner" method throws an `ArgumentOutOfRangeException`,
that's what you'll see too. It's not ideal, but it's a reasonable balance. You should almost never be catching any
of those exceptions explicitly anyway, so hopefully it's just a matter of being aware that if one of those exceptions
is thrown from the depths of Noda Time, it's probably due to a value being out of range somewhere.

As far as possible, we try to ensure that if you try to perform an operation which *can* succeed, it *will* succeed.
This is *not* guaranteed in all cases, however. For example, consider adding a `Period` consisting of -1 year and 365 days
to -9998-01-01. The "logical" result is a no-op, but as `Period` addition is performed one unit at a time, it will fail
on the addition of the -1 year.

<sup>1</sup> I'm not claiming that it *is* bug free, but I can't predict what will happen if there are bugs.