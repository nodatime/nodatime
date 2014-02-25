---
layout: userguide
title: Installation
category: intro
weight: 30
---

Our primary distribution channel is [NuGet](http://nuget.org) with
three packages:

- [NodaTime](http://nuget.org/packages/NodaTime)
- [NodaTime.Testing](http://nuget.org/packages/NodaTime.Testing)
- [NodaTime.Serialization.JsonNet](http://nuget.org/packages/NodaTime.Serialization.JsonNet)

Alternatively, source and binary downloads are available on the
[project download page][downloads].

[downloads]: http://nodatime.org/downloads/

See the ["Building and testing"][building] section in the developer guide for
instructions on building Noda Time from source.

[building]: http://nodatime.org/developer/building.html

System requirements
-------------------

From release 1.1 onwards, there are two builds of Noda Time: the desktop version and the Portable Class Library version.

The desktop version requires .NET 3.5 (client profile). This build also supports Mono, [with some caveats](mono.html).

The PCL build is configured to support:

- .NET Framework 4 and higher
- Silverlight 4 and higher
- Windows Phone 7 and higher
- .NET for Windows Store apps

The PCL build also appears to work with Xamarin.iOS and Xamarin.Android apps, but that is implicit in the set of configured targets. This has not been extensively tested, although we do not particularly expect to see significantly different issues in Xamarin applications to those seen under Mono in general.

Noda Time does *not* support XBox 360 or Silverlight 3, and it's unlikely that we'd ever want to introduce support
for these. (It's more likely that over time, we'll drop support for Silverlight - but not imminently, of course.)
See the [limitations](limitations.html) page for differences between the PCL build and the desktop build.

The NodaTime.Serialization.JsonNet assembly is built and tested against Json.NET version 4.5.11. It's likely that any version
of Json.NET from 4.5.0 onwards will work with Noda Time, but we'd recommend using at least 4.5.11. As far as we know, there
have been no breaking changes in Json.NET after that which affect Noda Time, but semantic versioning rules suggest that it
would at least be *possible* for later major versions to cause issues. If you discover any such problem, please report it to the
[Noda Time mailing list](http://groups.google.com/group/noda-time).

Package contents and getting started
------------------------------------

Everything you need to *use* Noda Time is contained in the NodaTime package. The NodaTime.Testing package is designed
for testing code which uses Noda Time. See the [testing guide](testing.html) for more information. It is expected
that production code will only refer to the `NodaTime.dll` assembly, and that's all that's required at execution time.
This assembly includes the [TZDB database](tzdb.html) as an embedded resource.

For Json.NET serialization, the NodaTime.Serialization.JsonNet package (containing a single assembly of the same name) is 
required, as well as an appropriate version of Json.NET itself. There is a NuGet dependency from NodaTime.Serialization.JsonNet
to the newtonsoft.json package, so if you're using NuGet you just need to refer to NodaTime.Serialization.JsonNet and an 
appropriate version of Json.NET will be installed automatically. See the [serialization guide](serialization.html) for more
information on using Noda Time with Json.NET.

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
