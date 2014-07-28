---
layout: userguide
title: Recipes
category: core
weight: 1035
---

New users can sometimes be a little overwhelmed by Noda Time, and want examples. This
page collects a lot of them in one place. A lot of these are small, because once you
know what operation you're trying to achieve, Noda Time makes it very easy to express
that... but it can still be tricky finding that if you're new to the API.

These examples typically use explicitly-typed local variables so that you can tell
all the types involved easily. You can use implicitly-typed local variables (`var`)
in your own code, of course. All examples assume a `using` directive of `using NodaTime;`.

TODO: more examples, and use C# Pad...

How old is Jon?
====

Jon was born on June 19th, 1976 (Gregorian). How old is he now, in the UK time zone?

    LocalDate birthDate = new LocalDate(1976, 6, 19);
    IClock clock = SystemClock.Instance;
    DateTimeZone zone = DateTimeZoneProviders.Tzdb["Europe/London"];
    LocalDate today = clock.GetCurrentInstant().InZone(zone).Date;
    Period age = Period.Between(birthDate, today);
    Console.WriteLine("Jon is: {0} years, {1} months, {2} days old.",
                      age.Years, age.Months, age.Days);

How can I get to the start or end of a month?
====

Use the [`DateAdjusters`](noda-type://NodaTime.DateAdjusters) factory class to obtain a suitable adjuster, and then either apply it
directly (it's just a `Func<LocalDate, LocalDate>`) or use the `With` method to apply it in a fluent
style:

    LocalDate today = new LocalDate(2014, 6, 27);
    LocalDate endOfMonth = today.With(DateAdjusters.EndOfMonth); // 2014-06-30

Date adjusters also work with `LocalDateTime` and `OffsetDateTime` values. For example:

    LocalDateTime now = new LocalDate(2014, 6, 27, 7, 14, 25);
    LocalDateTime startOfMonth = now.With(DateAdjusters.StartOfMonth); // 2014-06-01T07:14:25

How can I truncate a time to a particular unit?
====

Use the [`TimeAdjusters`](noda-type://NodaTime.TimeAdjusters) factory class to obtain a suitable adjuster, which can be applied to a
`LocalTime`, `LocalDateTime` or `OffsetDateTime`:

    LocalDateTime now = new LocalDate(2014, 6, 27, 7, 14, 25, 500);
    LocalDateTime now = now.With(TimeAdjusters.TruncateToMinute); // 2014-06-27T07:14:00
