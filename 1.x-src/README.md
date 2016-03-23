1.x source directory
===

This directory does not contain the source code for Noda Time 1.x;
that is on separate branches. Instead, it contains the source code
for packages which are still active but target 1.x - initially, that
just consists of the `NodaTime.Timezones.Tzdb` package which is updated
whenever IANA releases a new set of data.

Unfortunately this cannot easily live within the `src` directory as
the package name is inferred from the directory name, and we want
the same name for both 1.x and 2.x.