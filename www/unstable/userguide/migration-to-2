---
layout: userguide
title: Migrating from 1.x to 2.0
category: core
weight: 3010
---

Noda Time 2.0 contains a number of breaking changes. If you have a project which uses Noda Time
1.x and are considering upgrading to 2.0, please read the following migration guide carefully.
In particular, there are some changes which are changes to execution-time behaviour, and won't show up as compile-time errors.

Obsolete members
====

A few members in 1.x were already marked as obsolete, and they have now been removed. Code using 
these members will no longer compile. Two of these were simple typos in the name - fixing code 
using these is simply a matter of using the correct name instead:

- `Era.AnnoMartyrm` should be `Era.AnnoMartyrum`
- `Period.FromMillseconds` should be `Period.FromMilliseconds`

In addition, `DateTimeZoneProviders.Default` has been removed. It wasn't the default in any Noda 
Time code, and it's clearer to use the `DateTimeZoneProviders.Tzdb` member, which the `Default`
member was equivalent to anyway.
