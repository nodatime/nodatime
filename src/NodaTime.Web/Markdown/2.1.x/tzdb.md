@Title="Updating the time zone database"

Noda Time comes with a version of the
[tz database](https://www.iana.org/time-zones) (also known as the IANA Time Zone
database, or zoneinfo or Olson database), which is now hosted by IANA. This
database changes over time, as countries decide to change their time zone
rules.  As new versions of Noda Time are released, the version of tzdb will be
updated. However, you may wish to use a new version of tzdb *without* changing
which version of Noda Time you're using. This documentation tells you how to do
so.

Noda Time's main library doesn't read tzdb text files directly - it uses a binary form which is the output of `NodaTime.TzdbCompiler` - another
part of the Noda Time project. This saves both space (the 2013h version takes about 125K when compiled) and
time, as the binary form contains various precomputed transitions. The file also contains a mapping from Windows time zone names
to tzdb time zone IDs, primarily so that Noda Time can map the local time zone reported by `TimeZoneInfo` to a tzdb time zone,
and (from Noda Time 1.1 onwards) TZDB location data.

In Noda Time 1.0, the data was stored in .NET resources. This became awkward in a number of ways, not least because of
the lack of full resource support in Portable Class Libraries. In 1.1, a new file format was introduced, along with methods
to read this format from any stream. Support for the resource
format has been removed from Noda Time 2.0.

For more details on the exact formats, please see the [documentation in the developer guide](/developer/tzdb-file-format).

Obtaining and using a "NodaZoneData" file
=========================================

Update the NodaTime NuGet package
-----------------------------------

From March 2017 onwards, we aim to update NuGet packages with new
TZDB data shortly after each TZDB release, each update counting as a patch release.
This has the disadvantage that it's less clear when a patch release is really just TZDB data vs when
it's a regular bug-fix, but makes it much simpler for users. Simply update the NuGet package
version you're using, and you will get the new data.

All minor versions from 1.3 onwards are updated. Users still using versions older than 1.3
are encouraged to migrate, at least to 1.3.

Fetching a NodaZoneData file from nodatime.org
----------------------------------------------

NodaZoneData files are [available from nodatime.org](/tzdb/)
and contain compiled versions of TZDB from 2013h onwards.

These can be downloaded and used with any Noda
Time 1.1+ binary, so you don't need to update to the latest version
of Noda Time in order to get the latest version of TZDB, and you
don't have to build the file yourself either.

The URL [http://nodatime.org/tzdb/latest.txt](/tzdb/latest.txt)
returns a plaintext response containing the URL of the latest NZD file.
This may be used for automation.

Using a NodaZoneData file
-------------------------

Typically you'll want to use the newly-created resource file as the default time zone provider, across your whole application.
While it's possible to have multiple time zone providers in play at a time, that's a very rare scenario. Using a resource
file is relatively straightforward:

- Open a stream to the file
- Create a [`TzdbDateTimeZoneSource`][TzdbDateTimeZoneSource] with the stream, using the static `FromStream` method
- Create [`DateTimeZoneCache`][DateTimeZoneCache] with the source
- Use that cache (usually by way of dependency injection as an `IDateTimeZoneProvider`) wherever you need time zone information

Here's some sample code for the first three steps above:

```csharp
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.IO;

public class CustomTzdb
{
    static void Main()
    {
        IDateTimeZoneProvider provider;
        // Or use Assembly.GetManifestResourceStream for an embedded file
        using (var stream = File.OpenRead("tzdb-2013h.nzd"))
        {
            var source = TzdbDateTimeZoneSource.FromStream(stream);
            provider = new DateTimeZoneCache(source);
        }
        Console.WriteLine(provider.SourceVersionId);
    }
}
```

The stream is fully read in the call to `TzdbDateTimeZoneSource.FromStream`, so disposing of it afterwards (as shown above) doesn't
affect the source you've created. The stream can come from anywhere - typically it would either be a standalone file in the file
system, or an embedded resource file within one of your assemblies. You certainly *could* fetch it across a network, if you wanted.
(It is read sequentially from start to end - no seeking is required.)


Building a NodaZoneData file
----------------------------

This is very rarely required, given the usual short time between TZDB releases and both nodatime.org
and the NuGet packages being updated. There is no release of the NodaTime.TzdbCompiler; you will need
to fetch and build the source code.

1. Find the link to the [latest tzdb release](https://www.iana.org/time-zones), e.g.
   https://www.iana.org/time-zones/repository/releases/tzdata2015e.tar.gz
2. Determine the Windows mapping file you want to use, or let NodaTime.TzdbCompiler do it for you
   with the versions supplied with the Noda Time source in the `data\cldr` directory. If these are
   out of date, you can download a new file from [CLDR](http://cldr.unicode.org/).
3. Run NodaTime.TzdbCompiler. I'd suggest leaving it in its build directory and running it like this:

```bat
path\to\NodaTime.TzdbCompiler.exe -s tzdb-url -w windows-file-or-directory -o path\to\output.nzd
```

The `-s` argument can be an unpacked archive directory, a local archive, or a remote archive. The `-w`
argument can be a path to a single file, or a directory containing multiple Windows Zones XML files, in which
case the most appropriate one will be selected automatically.

[TzdbDateTimeZoneSource]: noda-type://NodaTime.TimeZones.TzdbDateTimeZoneSource
[DateTimeZoneCache]: noda-type://NodaTime.TimeZones.DateTimeZoneCache
