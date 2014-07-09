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
date from the start of the minimum year to the end of the maximum year is valid.

<table>
	<thead>
		<tr>
			<th>Calendar</th>
			<th>Min year</th>
			<th>Max year</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>Gregorian/ISO</td>
			<td>-9998</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Julian</td>
			<td>-9997</td>
			<td>9997</td>
		</tr>
		<tr>
			<td>Islamic</td>
			<td>1</td>
			<td>9664</td>
		</tr>
		<tr>
			<td>Persian</td>
			<td>1</td>
			<td>9376</td>
		</tr>
		<tr>
			<td>Hebrew</td>
			<td>1</td>
			<td>9999</td>
		</tr>
		<tr>
			<td>Coptic</td>
			<td>1</td>
			<td>9714</td>
		</tr>		
	</tbody>
</table>

TODO: Talk about Duration, Instant etc.