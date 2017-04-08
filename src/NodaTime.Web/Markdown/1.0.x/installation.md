@Title="Installation"

Our primary distribution channel is [NuGet](https://www.nuget.org/) with
two packages:

- [NodaTime](https://www.nuget.org/packages/NodaTime)
- [NodaTime.Testing](https://www.nuget.org/packages/NodaTime.Testing)

Alternatively, source and binary downloads are available on the
[project download page][downloads].

[downloads]: /downloads/

An experimental assembly to support Json.NET is also available (in source form
only); see the [serialization guide](serialization) for more information.

See the ["Building and testing"][building] section in the developer guide for
instructions on building Noda Time from source.

[building]: /developer/building

System requirements
-------------------

Currently Noda Time requires .NET 3.5 (client profile). It supports
Mono ([with some caveats](mono)) but does *not* support
Silverlight (or, by extension, Windows Phone 7). We'd like to
support these in the future, but it will involve a non-trivial
amount of work. The same goes for creating a Portable Class Library project,
although that may be simpler.

Package contents and getting started
------------------------------------

Everything you need to *use* Noda Time is contained in the NodaTime package. The NodaTime.Testing package is designed
for testing code which uses Noda Time. See the [testing guide](testing) for more information. It is expected
that production code will only refer to the `NodaTime.dll` assembly, and that's all that's required at execution time.
This assembly includes the [TZDB database](tzdb) as an embedded resource.

Everything within the NodaTime assembly is in the NodaTime namespace or a "child" namespace. After adding a reference to
the main assembly (either directly via the file system or with NuGet) and including an appropriate `using` directive, you should
be able to start using Noda Time immediately, with no further effort.
