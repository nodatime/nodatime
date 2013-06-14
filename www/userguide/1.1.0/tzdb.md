---
layout: userguide
title: Updating the time zone database in Noda Time
category: library
weight: 130
---

Noda Time comes with a version of the [tzdb](http://www.iana.org/time-zones) (aka zoneinfo) database, which is
now hosted by IANA. This database changes over time, as countries decide to change their time zone rules.
As new versions of Noda Time are released, the version of tzdb will be updated. However, you may wish to use
a new version of tzdb *without* changing which version of Noda Time you're using. This documentation tells you how
to do so.

Noda Time's main library doesn't read tzdb text files directly - it uses a binary form which is the output of `ZoneInfoCompiler` - another
part of the Noda Time project. This saves both space (the 2012j version takes about 110K when compiled, or about 690K in text form) and
time, as the binary form contains various precomputed transitions. The file also contains a mapping from Windows time zone names
to tzdb time zone IDs, primarily so that Noda Time can map the local time zone reported by `TimeZoneInfo` to a tzdb time zone.

In Noda Time 1.0, the data was stored in .NET resources. This became awkward in a number of ways, not least because of
the lack of full resource support in Portable Class Libraries. In 1.1, a new file format was introduced, along with methods
to read this format from any stream. If you're not using 1.0, we strongly advise you to use the new format; support for the resource
format will be removed in Noda Time 2.0.

For more details on the exact formats, please see the [documentation in the developer guide](http://noda-time.googlecode.com/hg/docs/developer/tzdb-file-format.html)

Creating and using a "NodaZoneData" file (1.1+ format)
======================================================

Building a NodaZoneData file
----------------------------

1. Download the [latest tzdb release](http://www.iana.org/time-zones)
2. Unpack the tar.gz file - you may need to download extra tools for this; [7-Zip](http://www.7-zip.org/) can cope with .tar.gz
   files for example, and I'd expect other zip tools to do so too. You should end up with a directory containing files such
   as "america", "africa", "etcetera".
3. Ideally, rename the directory to match the version number, e.g. "2012j". The directory name will be used in the version ID
   reported by the time zone provider later.
4. Find the Windows mapping file you want to use. Currently, I'd recommend using the version supplied with the Noda Time source
   in ZoneInfoCompiler\Data\winmap in a file beginning "windowsZones". This file comes from [CLDR](http://cldr.unicode.org).
5. Run ZoneInfoCompiler. I'd suggest leaving it in its build directory and running it like this:

        path\to\ZoneInfoCompiler.exe -s path\to\tzdb-files -w path\to\windowsMapping-file.xml -o path\to\output.nzd -t NodaZoneData

 For example, rebuilding the 2012j data from Noda Time itself, starting in the ZoneInfoCompiler directory:

        bin\Release\ZoneInfoCompiler -s Data\2012j -w Data\winmap\windowsZones-21.xml -o tzdb-2012j.nzd -t NodaZoneData

The NodaZoneData format is the default output format, so you can omit the final `-t NodaZoneData` if you wish; you may prefer
to be explicit for clarity.

As an alternative, if there's enough demand, we may well provide pre-built files in the Noda Time project download section.
It's worth knowing the above steps, however, in case you wish to use a cut-down set of time zones for resource-constrained environments.

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
            using (var stream = File.OpenRead("tzdb-2012j.nzd"))
            {
                var source = TzdbDateTimeZoneSource.FromStream(stream);
                provider = new DateTimeZoneCache(source);
            }
            Console.WriteLine(provider.SourceVersionId);
        }
    }

The stream is fully read in the call to `TzdbDateTimeZoneSource.FromStream`, so disposing of it afterwards (as shown above) doesn't
affect the source you've created. The stream can come from anywhere - typically it would either be a standalone file in the file
system, or an embedded resource file within one of your assemblies. You certainly *could* fetch it across a network, if you wanted.
(It is read sequentially from start to end - no seeking is required.)

Creating and using a resource file (legacy format)
==================================================

Building the resource file
--------------------------

1. Download the [latest tzdb release](http://www.iana.org/time-zones)
2. Unpack the tar.gz file - you may need to download extra tools for this; [7-Zip](http://www.7-zip.org/) can cope with .tar.gz
   files for example, and I'd expect other zip tools to do so too. You should end up with a directory containing files such
   as "america", "africa", "etcetera".
3. Ideally, rename the directory to match the version number, e.g. "2012j". The directory name will be used in the version ID
   reported by the time zone provider later.
4. Find the Windows mapping file you want to use. Currently, I'd recommend using the version supplied with the Noda Time source
   in ZoneInfoCompiler\Data\winmap in a file beginning "windowsZones". This file comes from [CLDR](http://cldr.unicode.org).
5. Run ZoneInfoCompiler. I'd suggest leaving it in its build directory and running it like this:

        path\to\ZoneInfoCompiler.exe -s path\to\tzdb-files -w path\to\windowsMapping-file.xml -o path\to\output.resources -t Resource

 For example, rebuilding the 2012j data from Noda Time itself, starting in the ZoneInfoCompiler directory:

        bin\Release\ZoneInfoCompiler -s Data\2012j -w Data\winmap\windowsZones-21.xml -o tzdb-2012j.resources -t Resource

As an alternative, if there's enough demand, we may well provide pre-built resource files in the Noda Time project download section.
It's worth knowing the above steps, however, in case you wish to use a cut-down set of time zones for resource-constrained environments.

Using a compiled resource file
------------------------------

Typically you'll want to use the newly-created resource file as the default time zone provider, across your whole application.
While it's possible to have multiple time zone providers in play at a time, that's a very rare scenario. Using a resource
file is relatively straightforward:

- Create a [`ResourceSet`](http://msdn.microsoft.com/en-us/library/t15hy0dt.aspx) from the file
- Create a [`TzdbDateTimeZoneSource`][TzdbDateTimeZoneSource] with the `ResourceSet`
- Create [`DateTimeZoneCache`][DateTimeZoneCache] with the source 
- Use that cache (usually by way of dependency injection as an `IDateTimeZoneProvider`) wherever you need time zone information

Here's some sample code for the first three steps above:

    using NodaTime;
    using NodaTime.TimeZones;
    using System;
    using System.Resources;

    public class CustomTzdb
    {
        static void Main()
        {
            var resourceSet = new ResourceSet("tzdb-2012j.resources");
            var source = new TzdbDateTimeZoneSource(resourceSet);
            IDateTimeZoneProvider provider = new DateTimeZoneCache(source);
            Console.WriteLine(provider.SourceVersionId);
        }
    }

You may be surprised that `TzdbDateTimeZoneSource` doesn't implement `IDisposable` even though `ResourceSet` does. `TzdbDateTimeZoneSource`
will never close or dispose the resource set it's given - it doesn't assume ownership of it. However, it will fail if you dispose the
resource set and then ask for a time zone which hasn't yet been loaded. If you wish to have a completely disconnected time zone provider,
I'd recommend loading the contents of the file into a `MemoryStream` and passing *that* to the `ResourceSet` constructor. Otherwise, in
most cases it probably isn't a big deal to have the handle to the resource file open throughout the lifetime of the application.

[TzdbDateTimeZoneSource]: noda-type://NodaTime.TimeZones.TzdbDateTimeZoneSource
[DateTimeZoneCache]: noda-type://NodaTime.TimeZones.DateTimeZoneCache
