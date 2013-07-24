---
layout: userguide
title: Unit testing with Noda Time
category: library
weight: 110
---

This page is not about how Noda Time itself is tested - it's about how you to test code
which *uses* Noda Time.

NodaTime.Testing
----------------

Firstly, get hold of the [NodaTime.Testing](http://nuget.org/packages/NodaTime.Testing) assembly. It's currently fairly
small, but it will no doubt grow - and it will make your life much easier. The purpose of the assembly is to provide
easy-to-use test doubles which can be used instead of the real implementations.

Dependencies
------------

While you *can* use Noda Time without dependency injection, it will make your code harder to test. Noda Time has
no particular support for any specific dependency injection framework, but should be easy to configure with any
reasonably-powerful implementation. (If it's not, please file a bug report.)

The most obvious dependency is a clock - an implementation of [`NodaTime.IClock`](noda-type://NodaTime.IClock),
which simply provides "the current date and time" (as an `Instant`, given that the concept of "now" isn't
inherently bound to any time zone or calendar). The [`FakeClock`](noda-type://NodaTime.Testing.FakeClock) can
be set to any given instant, advanced manually, or set to advance a given amount each time it's accessed. The production
environment should normally inject the singleton [`SystemClock`](noda-type://NodaTime.SystemClock) instance which simply
uses `DateTime.UtcNow` behind the scenes.

For code which is sensitive to time zone fetching, an [`IDateTimeZoneProvider`](noda-type://NodaTime.IDateTimeZoneProvider) can
be injected. There are currently no test doubles for this interface - please contact the mailing list with requirements
if to give us feedback on exactly what you'd like provided here. The production environment should usually be
configured with one of the providers in [`DateTimeZoneProviders`](noda-type://NodaTime.DateTimeZoneProviders).

For time zones themselves, a fake implementation representing a time zone with a single transition between different offsets
is available as [`SingleTransitionDateTimeZone`](noda-type://NodaTime.Testing.TimeZones.SingleTransitionDateTimeZone). Creating
a time zone with no transitions at all is simple via `DateTimeZone.ForOffset`.
