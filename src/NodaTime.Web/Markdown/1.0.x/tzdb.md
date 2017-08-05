@Title="Updating the time zone database"

Noda Time comes with a version of the [tzdb](https://www.iana.org/time-zones) (aka zoneinfo) database, which is
now hosted by IANA. This database changes over time, as countries decide to change their time zone rules.
As new versions of Noda Time are released, the version of tzdb will be updated. However, you may wish to use
a new version of tzdb *without* changing which version of Noda Time you're using. This documentation tells you how
to do so.

"Compiling" the database to a resource file
-------------------------------------------

Noda Time's main library doesn't read tzdb text files directly - it uses a binary form which is the output of `ZoneInfoCompiler` - another
part of the Noda Time project. This saves both space (the 2012c version takes about 189K when compiled, or about 598K in text form) and
time, as the binary form contains various precomputed transitions. The resource file also contains a mapping from Windows time zone names
to tzdb time zone IDs, primarily so that Noda Time can map the local time zone reported by `TimeZoneInfo` to a tzdb time zone.

`ZoneInfoCompiler` is not currently provided in binary form in NuGet packages (although this decision can easily be revisited if there's
enough demand). You'll need to build it yourself - which should be as simple as getting hold of a Noda Time source distribution (the latest
for your binary's major version number should be good enough; we won't change the format of the resource file without bumping the version
number) and building the whole solution. You'll end up with binaries in ZoneInfoCompiler/bin/Debug (or Release).

Steps
=====

1. Download the [latest tzdb release](https://www.iana.org/time-zones)
2. Unpack the tar.gz file - you may need to download extra tools for this; [7-Zip](http://www.7-zip.org/) can cope with .tar.gz
   files for example, and I'd expect other zip tools to do so too. You should end up with a directory containing files such
   as "america", "africa", "etcetera".
3. Ideally, rename the directory to match the version number, e.g. "2012c". The directory name will be used in the version ID
   reported by the time zone provider later.
4. Find the Windows mapping file you want to use. Currently, I'd recommend using the version supplied with the Noda Time source
   in ZoneInfoCompiler\Data\winmap in a file beginning "windowsZones". This file comes from [CLDR](http://cldr.unicode.org/).
5. Run ZoneInfoCompiler. I'd suggest leaving it in its build directory and running it like this:

```bat
path\to\ZoneInfoCompiler.exe -s path\to\tzdb-files -w path\to\windowsMapping-file.xml -o path\to\output.resources -t Resource
```

For example, rebuilding the 2012c data from Noda Time itself, starting in the ZoneInfoCompiler directory:

```bat
bin\Release\ZoneInfoCompiler -s Data\2012c -w Data\winmap\windowsZones-21.xml -o tzdb-2012c.resources -t Resource
```

As an alternative, if there's enough demand, we may well provide pre-built resource files in the Noda Time project download section.
It's worth knowing the above steps, however, in case you wish to use a cut-down set of time zones for resource-constrained environments.

Using a compiled resource file
------------------------------

Typically you'll want to use the newly-created resource file as the default time zone provider, across your whole application.
While it's possible to have multiple time zone providers in play at a time, that's a very rare scenario. Using a resource
file is relatively straightforward:

- Create a [`ResourceSet`](https://msdn.microsoft.com/en-us/library/t15hy0dt.aspx) from the file
- Create a [`TzdbDateTimeZoneSource`][TzdbDateTimeZoneSource] with the `ResourceSet`
- Create [`DateTimeZoneCache`][DateTimeZoneCache] with the source
- Use that cache (usually by way of dependency injection as an `IDateTimeZoneProvider`) wherever you need time zone information

Here's some sample code for the first three steps above:

```csharp
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Resources;

public class CustomTzdb
{
    static void Main()
    {
        var resourceSet = new ResourceSet("tzdb-2012c.resources");
        var source = new TzdbDateTimeZoneSource(resourceSet);
        IDateTimeZoneProvider provider = new DateTimeZoneCache(source);
        Console.WriteLine(provider.SourceVersionId);
    }
}
```

You may be surprised that `TzdbDateTimeZoneSource` doesn't implement `IDisposable` even though `ResourceSet` does. `TzdbDateTimeZoneSource`
will never close or dispose the resource set it's given - it doesn't assume ownership of it. However, it will fail if you dispose the
resource set and then ask for a time zone which hasn't yet been loaded. If you wish to have a completely disconnected time zone provider,
I'd recommend loading the contents of the file into a `MemoryStream` and passing *that* to the `ResourceSet` constructor. Otherwise, in
most cases it probably isn't a big deal to have the handle to the resource file open throughout the lifetime of the application.

[TzdbDateTimeZoneSource]: noda-type://NodaTime.TimeZones.TzdbDateTimeZoneSource
[DateTimeZoneCache]: noda-type://NodaTime.TimeZones.DateTimeZoneCache
