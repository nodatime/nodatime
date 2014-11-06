---
layout: userguide
title: Trivia
category: advanced
weight: 3050
---

The history of time contains many amusing quirks. In many cases, Noda Time
doesn't make any attempt to support the oddities described below. In
particular, Noda Time takes an idealized view of calendar systems, ignoring
(for example) the fact that countries have switched between calendar systems
(and sometimes *invented* calendar systems) at various points in recent
history. Still, it's fun to think about what's happened over time, if only
to persuade you that it would be much worse if Noda Time *did* try to
support them...

More will be added as I discover them.

The Curious Disappearance of December 30th (Samoa)
---

In 2011, Samoa (time zone ID `Pacific/Apia`) decided to change their UTC
offset from UTC-10 to UTC+14. This means they went from being the
western-most time zone (the further behind UTC) to the eastern-most time
zone (the furthest ahead of UTC). This took place at midnight local time at
the end of December 29th, so the local time for the few seconds around this
was:

- 2011-12-29T23:59:58-10
- 2011-12-29T23:59:59-10
- 2011-12-31T00:00:00+14
- 2011-12-31T00:00:01+14
- 2011-12-31T00:00:02+14

As an act, it made a certain amount of sense: Samoa does a lot of trade with
Australia and New Zealand, and I can imagine it was a pain being a day
behind them most of the time. Additionally, this means that hotels in Samoa
can offer the experience of being "the first to experience the New Year" for
tourists who are particularly keen on that sort of thing.

However, I question the wisdom of making the transition occur at midnight,
which made December 30th not occur at all. I suspect that a transition at
(say) 3am would have been cleaner, leading to a day lasting only three hours
followed by a day lasting twenty one hours. While this is mostly a problem
for computers, I would expect many automated systems to get thoroughly
confused by a day not happening at all. (The counter-argument is that it's
possible that a few automated systems would cope with two significantly
shorter days, too...)

`Pacific/Apia` isn't the only time zone to have made this sort of
transition, although it is the largest population to do so, as far as I'm
aware.

**Noda Time support:** fully supported!
`DateTimeZoneProviders.Tzdb["Pacific/Apia"].AtStartOfDay(new LocalDate(2011,
12, 30))` will throw a `SkippedTimeException` to indicate that the day never
occurred.

**Read more:**

- [BBC News](http://www.bbc.co.uk/news/world-asia-16351377)

February 30th? Only in Sweden...
---

Many people understand the leap year rule for the Gregorian calendar: a year
number denotes a leap year if it's divisible by 4, except when it's also
divisible by 100, except that what it's also divisible by 400 it *is* a leap
year again. This system was devised to keep the length of a calendar year
very close to the solar year. It's more accurate than the Julian calendar
system, which simply has every 4th year being a leap year. This is a little
bit like 3 being a very bad approximation for π, while 22/7 is more accurate
but also more complicated. In this case, it means that there are more leap
years in the Julian calendar system than in the Gregorian calendar system:
if you run the two side-by-side from the same start date, the Gregorian
system will gradually get further ahead of the Julian system by including
fewer February 29th days.

Many countries switched from the Julian calendar system to the Gregorian
calendar system in the 16th and 18th centuries.  Most made this change by
skipping ten or eleven days to "catch up" with where the Gregorian calendar
would be if the two calendars had both started with the same date at some
point in the 3rd century AD. For example, when Great Britain made the change
in September 1752, September 2nd was followed by September 14th.

Sweden did things a little differently. Rather than skipping lots of days
all in one go, they decided to skip February 29th altogether from 1700,
until they'd missed enough leap years to catch up to the Gregorian calendar.
So instead of being first correct in one calendar system and then the next
day correct in a different system (albeit leading to a very short month and
year), they'd be somewhere in between for about 40 years.

While this almost sounds like a reasonable plan (almost like the way that
[Google "smears" a leap second across a whole minute rather than simply
adding a
second](http://googleblog.blogspot.com/2011/09/time-technology-and-leaping-seconds.html)),
it went horribly wrong. In February 1700, Sweden skipped the leap year,
according to plan. That meant that they were one day ahead of the countries
still following the Julian calendar. Unfortunately, in the  same year -
although ironically *before* February 28th - the [Great Northern
War](http://en.wikipedia.org/wiki/Great_Northern_War) broke out. This
distracted Sweden from their calendrical machinations, and they *did* have a
leap year in 1704 and 1708.

At this point the Swedes apparently realized they were in a crazy situation,
and decided to go back to the Julian calendar. In order to do that, they had
to to insert the leap year they'd missed in 1700 back into their calendar.
They could have done so in a regular year (1710 for example), just making
that an extra leap year - but no, they decided to make 1712 a *double* leap
year, by giving February 30 days. Sweden made the final change to the
Gregorian calendar in the "normal" way of skipping 11 days, in February
1753.

So there we have it: February 30th was only a valid date once, and in one
place: 1712 in Sweden.

**Noda Time support:** Absolutely not. Noda Time doesn't support calendars
cutting over from the Julian to the Gregorian calendar system anyway, let
alone in such an odd way.

**Read more:**

- [Wikipedia](http://en.wikipedia.org/wiki/February_30)

The time in Greenwich at the Unix epoch
---

Many developers know that the Unix epoch is midnight on January 1st 1970
UTC<sup>1</sup>. Those who are aware that UTC itself was only introduced in
1972, and so to avoid applying it proleptically, decide to instead say that
it was midnight on January 1st 1970 at Greenwich, expecting that Greenwich
would be using GMT (Greenwich Mean Time) at the epoch.

Unfortunately, that's incorrect. At the Unix epoch, the time in London was
observing British *Standard* Time, which had been introduced in 1968. During
British Standard Time, the United Kingdom did not observe daylight saving,
but instead used UTC+1 for the entire period, until it was scrapped in 1971.

There are thus two British time zones abbreviated to "BST" - "British
Standard Time" and the rather more common "British Summer Time". Both have
an overall offset from UTC of 1 hour, but British Standard Time  has a
"standard" offset of 1 hour from UTC with no daylight saving component, and
British Summer Time has a standard offset of 0 hours from UTC, but one hour
of daylight saving time.

The Java standard library has a [known
bug](http://bugs.java.com/view_bug.do?bug_id=4832236) on this matter. When
formatting the Unix epoch in the `Europe/London` time zone, it correctly
gives output of 1am, but incorrectly states that the time zone abbreviation
is GMT.

**Noda Time support:** Correct, but only by virtue of writing this article.
Until late January 2014, the time zone data compiler considered a time zone
transition to be invalid unless it either changed the name or the wall
offset. The transition from summer time to standard time on October 27th
1968 was therefore ignored.

**Read more:**

- [BBC News](http://www.bbc.co.uk/news/uk-scotland-11643098)
- [History of Legal Time in
  Britain](http://www.polyomino.org.uk/british-time/)

<sup>1</sup> I'm studiously avoiding the differentiation between, UTC, UT,
UT0, TAI and the like. They make my head hurt, and in common usage time zone
offsets are given relative to UTC.

Skipping midnight
---

How would you define midnight? There are two obvious ways to do this:

- 12am at the start of a day
- The transition between one day and the next

You might expect these to be the same, but in reality they're not. In most
time zones, transitions (usually for daylight saving changes) occur in the
early morning - often 1am or 2am. However, a surprising number of time zones
have had at least *one* transition which started at midnight - typically
skipping forward, so that midnight didn't actually occur. I first came up
against the problem when modelling all-day calendar events as events that
started and ended at midnight. That's not so smart in São Paulo (as one
example) when midnight doesn't always exist - at least if we assume that
"midnight" is "12am".

The smart approach is to effectively use the second definition: when one day
becomes the next.

**Noda Time support:** Although `LocalTime` supports the idea of midnight,
if you want to convert a `LocalDate` into a `ZonedDateTime`, you'd typically
use
[`DateTimeZone.AtStartOfDay`](noda-method://NodaTime.DateTimeZone.AtStartOfDay)
which avoids the problem - so long as the date hasn't been skipped as we saw
earlier...

Which year is it anyway?
---

I mentioned earlier that Great Britain changed from the Julian calendar to
the Gregorian calendar in 1752... but the same Act of Parliament which made
that change also changed the year numbering system in most of Great Britain.
Before then, March 25th (Lady Day, celebrating the Annunciation) was deemed
to be the start of the year. So for example, in the early 1700s the dates
would run:

- December 31st 1735
- January 1st 1735
- ...
- March 24th 1735
- March 25th 1736
- March 26th 1736

Due to the Calendar (New Style) act, 1751 was the last year for which this
was true. December 31st 1751 was followed by January 1st 1752, leaving 1751
as a very short year (only 282 days). 1752 was also a short year (355 days)
due to the transition to the Gregorian calendar system skipping 11 days of
September.

For some reason, the Treasury decided it was too awkward to have tax years
that were short like this. Therefore in 1752 the tax year still started on
March 25th, and in 1753 it started on April 5th. In 1800, for reasons
we<sup>1</sup> haven't yet discovered, the Treasury decided it still liked
the Julian calendar's way of determining leap years, so around<sup>2</sup>
1800 the tax year shifted by another day, to April 6th - where it was
remained ever since.

All of this means that when you see a date in history, you need to be very
careful before you compare it with another date. To avoid *too* much
confusion, many historical dates are given using the Julian calendar, which
was being observed at the time (depending on the place, of course), while
using the "New Style" of year numbering. For example, [Henry
VIII](http://en.wikipedia.org/wiki/Henry_VIII_of_England) died on January
28th 1547 (New Style) or January 28th 1546 (Old Style). In my experience,
some web pages explicitly call out which numbering style they're using,
others record it as "January 28th 1546/1547" and hope that readers
understand why, and others just use the New Style without any explanation.

It's unclear to me at the moment whether before 1752, January 1st was still
celebrated as "the New Year". It wouldn't be the New Year according to the
obvious definition ("when the year number changes") but given that humanity
never seems to miss a chance for a party, I suspect there were festivities
anyway.

**Noda Time support**: Definitely not, although it would be *possible*...

**Read more:**

- [Wikipedia: Calender (New Style) act][wikipedia-newstyle-act]
- [Wikipedia: Old and New Style
  dates](http://en.wikipedia.org/wiki/Old_Style_and_New_Style_dates)
- [Malcolm Rowe's write-up in
  Google+](https://plus.google.com/+MalcolmRowe/posts/Bf5swesMPUV)

[wikipedia-newstyle-act]: http://en.wikipedia.org/wiki/Calendar_(New_Style)_Act_1750

<sup>1</sup> When I say "we" here, I really mean "Malcolm" who did most of
the work on understanding this issue based on Wikipedia and primary
sources...
<sup>2</sup> Possibly 1800, possibly 1801 - we haven't found
details yet, and it would depend on whether the Treasury was considering the
*tax* year for 1800, or the *calendar* year for 1800 to be a leap year.

Warning! Warning! This time zone is changing...
---

You might expect that time zone changes are planned years in advance, to
avoid confusion when planning events. Alas, it is not so. My first encounter
with a short amount of warning for a time zone change was in 2009. I was
working on Google Mobile Sync at the time, synchronizing calendars. One day,
a unit test failed. It was trying to infer a correct time zone from a very
broken representation, and it mistook the time zone in Greenland for the one
in Argentina. This was *partly* due to a bug in my own code, but the sudden
nature of the problem was due to Argentina giving a mere 11 days of notice
of their decision to cancel daylight saving time for 2009/2010. This was due
to a large amount of rainfall in 2009, leading to full hydroelectric dams.
The Argentine government considered that one of the major benefits of
daylight saving time was reduced electricity consumption, and so when the
dams were full, there was less reason to observe the change. This was the
first - and I hope the last - time that a unit test failure was caused by
the weather.

Since then, I've learned that this isn't as rare an occurrence as one would
hope - and that 11 days is quite a lot of notice compared with some
examples. In 2013, both Morocco and Libya gave less than a day's notice that
they were changing their time zone rules, leaving a certain amount of
confusion in their wake.

In 2011, Turkey gave a fair amount of warning (over two weeks!) that it
was delaying the DST transition, just by a single day, but the reason was
slightly odd - a nationwide exam that 1.5 million students were taking.

**Noda Time support:** Well, there are five steps which need to occur
between a change being announced and Noda Time supporting it in production:

1. The change is announced in a suitably credible manner
2. The change is committed within the TZDB source repository
3. The TZDB maintainers release a new version
4. I run a script to pull the latest version from TZDB, build it as an NZD
   file and push it to the Noda Time web site
5. The system in question pulls the latest version from the web site and
   [starts using it](tzdb.html)

As you can see, this depends on a lot of different people, so some delay is
inevitable. However, if your application is configured to poll the web site
reasonably often (once a day, for example) you're likely to see changes
applied within a week of them being announced (and of course, you can always
build the NZD file yourself). That's considerably quicker than Microsoft
typically deploys changes to the *Windows* time zone database...

**Read more:**

- [Argentina (announcement on 7th October 2009, due 18th
  October)](http://www.timeanddate.com/news/time/argentina-dst-2009-2010.html)
- [Morocco (September
  2013)](http://www.timeanddate.com/news/time/morocco-ends-dst-2013.html)
- [Libya (October
  2013)](http://www.timeanddate.com/news/time/libya-cancels-dst-switch-2013.html)
- [Turkey (March 2011)](http://www.timeanddate.com/news/time/turkey-starts-dst-2011.html)
