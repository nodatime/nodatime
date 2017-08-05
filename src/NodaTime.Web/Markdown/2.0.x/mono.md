@Title="Mono support"

[Mono](http://www.mono-project.com/) is an open source implementation of
the Common Language Infrastructure which runs on various platforms,
including Windows, Linux and OS X.

Noda Time runs on Mono, but its implementation of `TimeZoneInfo` is known to have
some problems, at least on Linux. (Earlier versions had significantly more issues.)
If you can use the TZDB time zone provider within Noda Time instead of the BCL one,
you're likely to encounter fewer problems.

Some issues around cultural information used for date/time formatting have been noticed
in the past. These appear to have improved. Unfortunately as Mono runs in so many environments, it's
hard to test everything - please report any errors you run into. Using the invariant culture and
custom format patterns should always work correctly; it's only when you need localization that
things get tricky.

The prebuilt binary packages can be used on Mono directly. To build from source,
See the ["Building and testing"][building] section in the developer guide.

[building]: /developer/building
