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
    LocalDate today = clock.Now.InUtc().Date;
    Period age = Period.Between(birthDate, today);
    Console.WriteLine("Jon is: {0} years, {1} months, {2} days old.",
                      age.Years, age.Months, age.Days);