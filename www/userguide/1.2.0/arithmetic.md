---
layout: userguide
title: Date and time arithmetic
category: advanced
weight: 70
---

There are two types of arithmetic in Noda Time: arithmetic on the
time line (in some sense "absolute" arithmetic), and calendrical arithmetic.
Noda Time deliberately separates the two, with the aim of avoiding
subtle bugs where developers may be tempted to mix concepts inappropriately.

Time Line Arithmetic
====================

The [`Instant`](noda-type://NodaTime.Instant) and
[`ZonedDateTime`](noda-type://NodaTime.ZonedDateTime) types both unambiguously
refer to a point in time (with the latter additionally holding time
zone and calendar information). We can add a length of time to that point to get
another point in time - but only if it's a truly fixed length of time, such as
"3 minutes". While a minute is a fixed length of time, a month isn't - so
the concept of adding "3 months" to an instant makes no sense. (Note
that Noda Time doesn't support leap seconds, otherwise even "3
minutes" wouldn't be a fixed length of time.)

These "fixed lengths of time" are represented in Noda Time with the
[`Duration`](noda-type://NodaTime.Duration) type, and they can be
added to either an `Instant` or `ZonedDateTime` using either the `+` operator
or `Plus` methods:

    Duration duration = Duration.FromMinutes(3);
    Instant now = SystemClock.Instance.Now;    
    Instant future = now + duration; // Or now.Plus(duration)
    
    ZonedDateTime nowInIsoUtc = now.InUtc();
    ZonedDateTime thenInIsoUtc = nowInIsoUtc + duration;

(There are also static `Add` and `Subtract` methods, the `-` operator and
the instance `Minus` method on both `Instant` and `ZonedDateTime`.)

Time line arithmetic is pretty simple, except you might not *always* get
what you expect when using `ZonedDateTime`, due to daylight saving transitions:

    DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
    // 12:45am on March 27th 2012
    LocalDateTime local = new LocalDateTime(2012, 3, 27, 0, 45, 00);
    ZonedDateTime before = london.AtStrictly(local);
    ZonedDateTime after = before + Duration.FromMinutes(20);
    
We start off with a *local* time of 12.45am. The time that we add is effectively
"experienced" time - as if we'd simply waited twenty minutes. However, at 1am on that day,
the clocks in the Europe/London time zone go forward by an hour - so we end up with a local
time of 2:05am, not the 1:05am you might have expected.

The reverse effect can happen at the other daylight saving transition, when clocks go backward
instead of forward: so twenty minutes after 1:45am could easily be 1:05am! So even though we
expose the concept of "local time" in `ZonedDateTime`, any arithmetic performed on it is computed
using the underlying time line.

Calendrical Arithmetic
======================

The other type of arithmetic you'll typically want to perform with
Noda Time doesn't involve elapsed time directly, or even truly fixed
lengths of time - it involves calendrical concepts such as months.
These operations involve the "local" date and time types, which are
associated with a
[`CalendarSystem`](noda-type://NodaTime.CalendarSystem), but not a
time zone:

- [`LocalDateTime`](noda-type://NodaTime.LocalDateTime)
- [`LocalDate`](noda-type://NodaTime.LocalDate)
- [`LocalTime`](noda-type://NodaTime.LocalTime)

(`LocalTime` doesn't actually have an associated calendar, as Noda
Time assumes that all calendars model times in the same way, but
it's clearly closely related to the other two types.)

Adding a single unit
--------------------

The simplest kind of arithmetic is adding some number of months,
years, days, hours etc to a value - a quantity of a single "unit",
in other words. This is easily achieved via the `PlusXyz` methods on
all of the local types:

    LocalDate date = new LocalDate(2012, 2, 21);
    date = date.PlusMonths(1); // March 21st 2012
    date = date.PlusDays(-1); // March 20th 2012
    
    LocalTime time = new LocalTime(7, 15, 0);
    time = time.PlusHours(3); // 10:15 am
    
    LocalDateTime dateTime = date + time;
    dateTime = dateTime.PlusWeeks(1); // March 27th 2012, 10:15am
    
All of these types are immutable of course: the `PlusXyz` methods
don't modify the value they're called on; they return a new value
with the new date/time.

Even adding or subtracting a single unit can introduce problems due
to unequal month lengths and the like. When adding a month or a year
would create an invalid date, the day-of-month is truncated. For
example:

    LocalDate date = new LocalDate(2012, 2, 29);
    LocalDate date1 = date.PlusYears(1); // February 28th 2013

    LocalDate date2 = date.PlusMonths(1).PlusDays(1); // March 30th 2012
    date2 = date2.PlusMonths(-1); // Back to February 29th 2012

`LocalTime` wraps around midnight transparently, but the same
operations on `LocalDateTime` will change the date appropriately too:

    LocalTime time = new LocalTime(20, 30, 0);
    time = time.PlusHours(6); // 2:30 am
    
    LocalDateTime dateTime = new LocalDate(2012, 2, 21) + time;
    dateTime = dateTime.PlusHours(-6); // 8:30pm on February 20th

Hopefully all of this is what you'd expect, but it's worth making
sure the simple cases are clear before moving on to more complex ones.

Adding a `Period`
-----------------

Sometimes you need a more general representation of the value to
add, which is where [`Period`](noda-type://NodaTime.Period) comes
in. This is essentially just a collection of unit/value pairs - so
you can have a period of "1 month and 3 days" or "2 weeks and 10
hours". Periods aren't normalized, so a period of "2 days" is not
the same as a period of "48 hours"; likewise if you ask for the
number of hours in a period of "1 day" the answer will be 0, not 24.

Single-unit periods can be obtained using the `FromXyz` methods, and
can then be added to the "local" types using the `+` operator or the
`Plus` method:

    LocalDateTime dateTime = new LocalDateTime(2012, 2, 21, 7, 48, 0);
    dateTime = dateTime + Period.FromDays(1) + Period.FromMinutes(1);
    dateTime = dateTime.Plus(Period.FromHours(1));

Adding a period containing date units to a `LocalTime` or adding a
period containing time units to a `LocalDate` will result in an
`ArgumentException`.

Periods can be combined using simple addition too:

    Period compound = Period.FromDays(1) + Period.FromMonths(1);
    
Again, this is very simple - the components in the two periods are
simply summed, with no normalization. Subtraction works in the same
way.

An alternative way of creating a period is to use [`PeriodBuilder`](noda-type://NodaTime.PeriodBuilder)
which is mutable, with a nullable property for each component:

    Period compound = new PeriodBuilder { Days = 1, Months = 1 }.Build();

Adding a compound period can sometimes give unexpected results if
you don't understand how they're handled, but the rule is extremely
simple: **One component is added at a time, starting with the most
significant, and wrapping / truncating at each step.**

It's easiest to think about where this can be confusing with an
example. Suppose we add "one month minus three days" to January 30th
2011:

    Period period = Period.FromMonths(1) - Period.FromDays(3);
    LocalDate date = new LocalDate(2011, 1, 30);
    date = date + period;
    
If you give this puzzle to a real person, they may well come up with
an answer of "February 27th" by waiting until the last moment to
check the validity. Noda Time will give an answer of February 25th,
as the above code is effectively evaluated as:

    Period period = Period.FromMonths(1) - Period.FromDays(3);
    LocalDate date = new LocalDate(2011, 1, 30);
    date = date + Period.FromMonths(1); // February 28th (truncated)
    date = date - Period.FromDays(3); // February 25th

The benefit of this approach is simplicity and predictability: when
you know the rules, it's very easy to work out what Noda Time will
do. The downside is that if you *don't* know the rules, it looks
like it's broken. It's possible that in a future version we'll
implement a "smarter" API (as a separate option, probably, rather
than replacing this one) - please drop a line to the mailing list if
you have requirements in this area.

Finding a period between two values
===================================

The opposite of this sort of arithmetic is answering questions such
as "How many days are there until my birthday?" or "How many years,
months, weeks, days, hours, minutes and seconds have I been married?"

Noda Time provides this functionality using `Period.Between`.
There are six overloads - two for each local type, with one using a
default set of units (year, month, day for dates; all time units for
times; year, month, day and all time units for date/times) and the
other allowing you to specify your own value. The units are specified
with the [`PeriodUnits`](noda-type://NodaTime.PeriodUnits) enum, and
can be combined using the `|` operator. So for example, to find out
how many "months and days" old I am at the time of this writing, I'd use:

    var birthday = new LocalDate(1976, 6, 19);
    var today = new LocalDate(2012, 2, 21);
    var period = Period.Between(birthday, today,
                                PeriodUnits.Months | PeriodUnits.Days);
    
    Console.WriteLine("Age: {0} months and {1} days",
                      period.Months, period.Days);

Just as when adding periods, computing a period works from the largest
specified unit down to the smallest, at each stage finding the appropriate
component value with the greatest magnitude so that adding it to the running
total doesn't "overshoot". This means that when computing a "positive" period
(where the second argument is later than the first) every component value will
be non-negative; when computing a "negative" period (where the second argument is
earlier than the first) every component value will be zero or negative.

Note that these rules can very easily give asymmetric results. For example, consider:

    // Remember that 2012 is a leap year...
    var date1 = new LocalDate(2012, 2, 28);
    var date2 = new LocalDate(2012, 3, 31);
        
    var period1 = Period.Between(date1, date2);
    var period2 = Period.Between(date2, date1);

Now `period1` is "1 month and 3 days" - when we add a month to `date1` we get to March 28th, and
then another 3 days takes us to March 31st. But `period2` is "-1 month and -1 day" - when we subtract
a month from `date2` we get to February 29th due to truncation, and then we only have to subtract
one more day to get to February 28th.

Again, this is easy to reason about and easy to implement. Contact the mailing list with
extra requirements if you have them.

Why doesn't this work with `ZonedDateTime`?
===========================================

All of this code using periods only works with the "local" types - notably there's
no part of the [`ZonedDateTime`](noda-type://NodaTime.ZonedDateTime) which mentions `Period`.
This is entirely deliberate, due to the complexities that time zones introduce. Every time
you perform a calculation on a `ZonedDateTime`, you may end up changing your offset from UTC.

So what does "12:30am plus one hour" mean when the hour between 1am and 2am is skipped?
Does it take us to 2:30am? That doesn't sound so bad - but when you add a year to a "midnight"
value and end up with 1am, that could be confusing... and there's even more confusion when you
think about ambiguous times. For example, supposed you have a `ZonedDateTime` representing
the *first* occurrence of 1:50am on a day when the clocks go back from 2am to 1am. What should
adding 20 minutes do? It could use the "elapsed" time, and end up with 1:10am (the second occurrence)
or it could end up with 2:10am (which would actually be 80 minutes later in elapsed time). None
of these options is particularly attractive.

Instead when you want to do calculations which *aren't* just based on a fixed number of ticks,
Noda Time forces you to convert to a local representation, perform all the arithmetic you want
there, then convert back to `ZonedDateTime` *once*, specifying how to handle ambiguous or
skipped times in the normal way. We believe this makes the API easier to follow and forces you
to think about the problems which you might otherwise brush under the carpet... but if you have
better suggestions, please raise them!

Currently Noda Time doesn't support arithmetic with [`OffsetDateTime`](noda-type://NodaTime.OffsetDateTime)
either, mostly because it's not clear what the use cases would be. You can always convert to either local or
zoned date/time values, perform arithmetic in that domain and convert back if necessary - but if you find
yourself in this situation, we'd love to hear about it on the Noda Time mailing list.

Days of the week
================

One aspect of calendar arithmetic which is often required but doesn't fit in anywhere
else is finding an appropriate day of the week. `LocalDateTime` and `LocalDate` both provide
`Next(IsoDayOfWeek)` and `Previous(IsoDayOfWeek)` methods for this purpose. Both give the
*strict* next/previous date (or date/time) falling on the given day of the week - so calling
`date.Next(IsoDayOfWeek.Sunday)` on a `date` which is *already* on a Sunday will return a date
a week later, for example. The methods on `LocalDateTime` leave the time portion unchanged.

Currently all the calendars supported by Noda Time use the ISO week; if we ever support a
calendar which doesn't, we'll see whether there's any need for a similar set of methods
operating on non-ISO week days.

----

See also:

- [The joys of date/time arithmetic](http://noda-time.blogspot.com/2010/11/joys-of-datetime-arithmetic.html)
