@Title="Week years"

Introducing week-years
===

Most of the time, dates are represented as a year, month and day in
a particular calendar system - occasionally with an era as well. An
alternative approach is to represent a date as a year, week,
and day-of-week.

For example, using the ISO-8601 rule, the year/month/day combination
"2016, February, 4" is equivalently year 2016, week 5,
Thursday. All of this is performed relative to a specific calendar
system.

However, under the same rule the year/month/day combination "2014,
December, 29th" is equivalently year **2015**, week 1, Monday: the
"year" part of a year/month/day representation isn't always the same
as the year part of the year/week/day-of-week representation for the
same date. To avoid ambiguity, I refer to the year part of a
year/week/day-of-week representation as a **week-year**.

The two representations are exactly equivalent: every day
can be represented in either form. Week-years are more widely used
in some countries than in others, typically in business or
educational settings.

There are different ways of mapping between year/month/day
and week-year/week/day-of-week representations, which we refer to as
**week-year rules**.

Noda Time features for week-years
===

As of Noda Time 2.0, week-year rules are entirely separate from calendar
systems. A rule is represented by the
[`IWeekYearRule`](noda-type://NodaTime.Calendars.IWeekYearRule)
interface, and built-in implementations are obtained via factory
members in the
[`WeekYearRules`](noda-type://NodaTime.Calendars.WeekYearRules)
static class.

Although the `GetWeeksInWeekYear` and `GetLocalDate` methods require
a `CalendarSystem`, we also provide extension methods in
[`WeekYearRuleExtensions`](noda-type://NodaTime.Calendars.WeekYearRuleExtensions)
that default to the ISO calendar system, delegating to the existing
`IWeekYearRule` methods. This reduces the burden of implementing
`IWeekYearRule`, but allows callers who are only using the ISO
calendar system to omit it.

There are two methods to convert from year-month-day representation
to week-based representation: `GetWeekYear` and `GetWeekOfWeekYear`,
both accepting a `LocalDate`. There is no equivalent to obtain the
day of week, as that does not change based on the rule or even on
the calendar system.

Noda Time 2.0 comes with two kinds of rule: ISO-like rules, and BCL
rules.

ISO-like rules
---

Let's start with the most common rule: `WeekYearRules.Iso`, which
implements the rule specified in ISO-8601. Under this rule:

- All weeks are Monday-to-Sunday
- If the week that contains the first day of the year has four days
  or more in that year, then the whole of that week is in week 1 of
  that year; otherwise, the whole of that week is in the final week
  of the previous year.

To put the "four days or more" rule in a different way: if the first
day of the calendar year is Monday, Tuesday, Wednesday or Thursday,
then the week containing that day is week 1 of the same year; if
the calendar year starts on a Friday, Saturday or Sunday, then that
week is in the previous week-year.

Examples:

- January 1st 2013 is a Tuesday, so 2013 week 1 runs from 2012-12-31
to 2013-01-06 inclusive
- January 1st 2014 is a Wednesday, so 2014 week 1 runs from 2013-12-30
to 2013-01-05 inclusive
- January 1st 2015 is a Thursday, so 2015 week 1 runs from
2014-12-29 to 2015-01-04 inclusive
- January 1st 2016 is a Friday, so **2015 week 53** runs from
2015-12-28 to 2016-01-03 inclusive, and 2016 week 1 runs from
2016-01-04 to 2016-01-10 inclusive.
- January 1st 2017 is a Sunday, so **2016 week 52** runs from
2016-12-26 to 2017-01-01 inclusive, and 2017 week 1 runs from
2017-01-02 to 2017-01-08 inclusive.

Although this is the canonical ISO-8601 rule, we can construct rules
which behave similarly, which I call "ISO-like" rules. These provide
two other axes of flexibility:

- First day of week. For example, if you want Sunday-to-Saturday
weeks, but still "four days or more" then the first day of the
calendar year is in week 1 if it's a Sunday, Monday, Tuesday or
Wednesday.
- The required number of days in the week containing the first day
of the calendar year to make that day part of week 1. For example,
with a Monday-to-Sunday week, and a "minimum days in first week"
value of 7, the first day of the calendar year will *only* be in
week 1 if it's a Monday. At the other extreme, a "minimum days
in first week" value of 1 means the first day of the calendar year
is *always* in week 1.

Use the `WeekYearRules.ForMinDaysInFirstWeek(int)` and
`WeekYearRules.ForMinDaysInFirstWeek(int, IsoDayOfWeek)` methods to
construct ISO-like rules. The first overload defaults to using Monday
as the first day of the week.

The rule returned by `WeekYearRules.Iso` property is equivalent to
the rule returned by
`WeekYearRules.ForMinDaysInFirstWeek(4, IsoDayOfWeek.Monday)`.

BCL rules
---

BCL rules are obtained via `WeekYearRules.FromBclRule`, which
accepts two parameters:

- A [`CalendarWeekRule`](https://msdn.microsoft.com/en-us/library/system.globalization.calendarweekrule)
- A [`DayOfWeek`](https://msdn.microsoft.com/en-us/library/system.dayofweek)

These mirror the second and third parameters to the
[`Calendar.GetWeekOfYear`](https://msdn.microsoft.com/en-us/library/system.globalization.calendar.getweekofyear)
method in the BCL.

It's easiest to think of the BCL rules as being equivalent to the
ISO-like rules in most respects, with a mapping from
`CalendarWeekRule` to "minimum number of days in first week" of:

- `FirstDay`: 1
- `FirstFourDayWeek`: 4
- `FirstFullWeek`: 7

There's then one subtle difference between ISO-like rules and
BCL-like rules:

> In an ISO-like rule, every week has exactly seven days. In a
BCL-like rule, if the start of the first week of a week-year is in
the previous calendar year, that week is split into two, so that the
week-year of a date is never more than the calendar year of the same
date.

It's easiest to see this in action with a couple of examples. The
BCL rule using `CalendarWeekRule.FirstFourDayWeek` and Monday as a
first day of the week is equivalent to `WeekYearRules.Iso`. Let's
see what happens around the turn of the year for two years. First
2014/2015:

<table>
  <thead>
    <tr>
      <td>Date</td>
      <td>Day of week</td>
      <td>ISO week</td>
      <td>BCL week</td>
  </thead>
  <tbody>
    <tr>
      <td>2014-12-28</td>
      <td>Sunday</td>
      <td>2014 week 52</td>
      <td>2014 week 52</td>
    </tr>
    <tr>
      <td>2014-12-29</td>
      <td>Monday</td>
      <td>2015 week 1</td>
      <td>2014 week 53</td>
    </tr>
    <tr>
      <td>2014-12-30</td>
      <td>Tuesday</td>
      <td>2015 week 1</td>
      <td>2014 week 53</td>
    </tr>
    <tr>
      <td>2014-12-31</td>
      <td>Wednesday</td>
      <td>2015 week 1</td>
      <td>2014 week 53</td>
    </tr>
    <tr>
      <td>2015-01-01</td>
      <td>Thursday</td>
      <td>2015 week 1</td>
      <td>2015 week 1</td>
    </tr>
    <tr>
      <td>2015-01-02</td>
      <td>Friday</td>
      <td>2015 week 1</td>
      <td>2015 week 1</td>
    </tr>
    <tr>
      <td>2015-01-03</td>
      <td>Saturday</td>
      <td>2015 week 1</td>
      <td>2015 week 1</td>
    </tr>
    <tr>
      <td>2015-01-04</td>
      <td>Sunday</td>
      <td>2015 week 1</td>
      <td>2015 week 1</td>
    </tr>
    <tr>
      <td>2015-01-05</td>
      <td>Monday</td>
      <td>2015 week 2</td>
      <td>2015 week 2</td>
    </tr>
  </tbody>
</table>

As you can see, 2015 started on a Thursday. In the ISO rule, that
means that the whole Monday-to-Sunday week is 2015 week 1. In the
BCL rule, Monday to Wednesday are in 2014 week 53, and Thursday to
Sunday are in 2015 week 1. For the BCL rule, 2014 week 53 and 2015
week 1 are "short" weeks.

Compare that with the 2015/2016 boundary:

<table>
  <thead>
    <tr>
      <td>Date</td>
      <td>Day of week</td>
      <td>ISO week</td>
      <td>BCL week</td>
  </thead>
  <tbody>
    <tr>
      <td>2015-12-27</td>
      <td>Sunday</td>
      <td>2015 week 52</td>
      <td>2015 week 52</td>
    </tr>
    <tr>
      <td>2015-12-28</td>
      <td>Monday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2015-12-29</td>
      <td>Tuesday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2015-12-30</td>
      <td>Wednesday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2015-12-31</td>
      <td>Thursday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2016-01-01</td>
      <td>Friday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2016-01-02</td>
      <td>Saturday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2016-01-03</td>
      <td>Sunday</td>
      <td>2015 week 53</td>
      <td>2015 week 53</td>
    </tr>
    <tr>
      <td>2016-01-04</td>
      <td>Monday</td>
      <td>2016 week 1</td>
      <td>2016 week 1</td>
    </tr>
  </tbody>
</table>

2016 starts on a Friday, which means there are only three days of
that Monday-to-Sunday week in 2016... which means that in both the
BCL and ISO rules, those days are in the final week of 2015. The
first week of 2016 starts on Monday January 4th, and the BCL and ISO
rules behave identically, with all weeks being a regular seven days.