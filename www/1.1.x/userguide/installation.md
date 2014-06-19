---
layout: userguide
title: Installation
category: intro
weight: 30
---

Our primary distribution channel is [NuGet](http://nuget.org) with
two packages:

- [NodaTime](http://nuget.org/packages/NodaTime)
- [NodaTime.Testing](http://nuget.org/packages/NodaTime.Testing)

Alternatively, source and binary downloads are available on the
[project download page][downloads].

[downloads]: http://nodatime.org/downloads/

An experimental assembly to support Json.NET is also available (in source form
only); see the [serialization guide](serialization.html) for more information.

See the ["Building and testing"][building] section in the developer guide for
instructions on building Noda Time from source.

[building]: http://nodatime.org/developer/building.html

System requirements
-------------------

As of release 1.1, there are two builds of Noda Time: the desktop version and the Portable Class Library version.

The desktop version requires .NET 3.5 (client profile). This build also supports Mono, [with some caveats](mono.html).

The PCL build is configured to support:

- .NET Framework 4 and higher
- Silverlight 4 and higher
- Windows Phone 7 and higher
- .NET for Windows Store apps

Noda Time does *not* support XBox 360 or Silverlight 3, and it's unlikely that we'd ever want to introduce support
for these. (It's more likely that over time, we'll drop support for Silverlight - but not imminently, of course.)
See the [limitations](limitations.html) page for differences between the PCL build and the desktop build.

Package contents and getting started
------------------------------------

Everything you need to *use* Noda Time is contained in the NodaTime package. The NodaTime.Testing package is designed
for testing code which uses Noda Time. See the [testing guide](testing.html) for more information. It is expected
that production code will only refer to the `NodaTime.dll` assembly, and that's all that's required at execution time.
This assembly includes the [TZDB database](tzdb.html) as an embedded resource.

Everything within the NodaTime assembly is in the NodaTime namespace or a "child" namespace. After adding a reference to
the main assembly (either directly via the file system or with NuGet) and including an appropriate `using` directive, you should
be able to start using Noda Time immediately, with no further effort.

Debugging
---------

As of version 1.1, the source code of Noda Time is published to [SymbolSource](http://www.symbolsource.org). You can configure
Visual Studio to automatically fetch the source code if you need to step into it when debugging your application. It takes a little
bit of setup, but there are [full instructions](http://www.symbolsource.org/Public/Home/VisualStudio) on the SymbolSource web site.
(The instructions aren't specific to Noda Time, so if you're
already using SymbolSource as one of your symbol servers, you just need to make sure you're not excluding Noda Time from the list of
modules to fetch.)

If you believe you've found a bug in Noda Time, using the SymbolSource version is likely to prove painful after a while - it's
much better to just fetch the [source code](https://code.google.com/p/noda-time/source/checkout) and build your own copy locally.
