---
layout: developer
title: Building and testing
---

## Visual Studio (Windows)

Noda Time is mostly developed on Visual Studio 2010 and Visual Studio 2012.
All versions of Visual Studio 2010 and 2012 which support C#, including Express editions,
should be able to build Noda Time. (Please contact the mailing list if this turns out
not to be the case.)

You'll also need the [.NET Framework SDK][dotnetsdk]. This library supports
version 2.0, 3.0, 3.5, 4 and 4.5 of the Framework. Install the appropriate
version or versions as defined by your needs.

[dotnetsdk]: http://msdn.microsoft.com/en-us/netframework/aa569263.aspx

To fetch the source code from the main Google Code repository, you'll need a
[Mercurial][] client. A good alternative for Microsoft Windows users is
[TortoiseHg][] which installs shell extensions so that Mercurial can be used
from the Windows Explorer.

[Mercurial]: http://mercurial.selenic.com/
[TortoiseHg]: http://tortoisehg.bitbucket.org/download/

To run the tests, you'll need [NUnit][] version 2.5.10 or higher.

[NUnit]: http://nunit.org/index.php?p=download

### Fetching and building

To fetch the source code, just clone the Google Code repository:

    > hg clone https://code.google.com/p/noda-time/

To build under Visual Studio, simply open the `src\NodaTime.sln` file.
To build with just the SDK and msbuild, run

    > msbuild src\NodaTime.sln /property:Configuration=Debug

Execute the tests using your favourite NUnit test runner. For example:

    > nunit-console src\NodaTime.Test\bin\Debug\NodaTime.Test.dll

(Include the other test DLLs should you wish to, of course.)

## Mono

*Note:* If you build Noda Time under Mono but execute it under the Microsoft
.NET 4 64-bit CLR, you may see exceptions around type initializers and
`RuntimeHelpers.InitializeArray`. We believe this to be due to a
[bug in .NET][ms-635365] which the Mono compiler happens to trigger. We
would recommend that you use a binary built by the Microsoft C# compiler if you
wish to run using this CLR.

[ms-635365]: http://connect.microsoft.com/VisualStudio/feedback/details/635365

We have tested Mono support using Mono 2.10.5 and 2.10.8.

To build Noda Time under Linux, you typically need to install the following
packages:

- mono-devel
- mono-xbuild

(These will add the other dependencies you need.)

Note that for Ubuntu specifically, you'll either need Ubuntu 11.10 (Oneiric) or
later, or work out how to install an unofficial backport; earlier versions of
Ubuntu [do not include a suitable version of Mono][MonoUbuntu].

[MonoUbuntu]: http://www.mono-project.com/DistroPackages/Ubuntu

To build Noda Time under OS X, [download][MonoDownload] the latest stable
release of Mono. Be sure to choose the developer package, not the smaller
runtime-only package.  To use the provided `Makefile`, you'll either need to
install [Xcode][xcode] or obtain `make` separately (for example, using
[osx-gcc-installer][] to install just the open-source parts of Xcode).

[MonoDownload]: http://www.mono-project.com/Download
[xcode]: https://developer.apple.com/xcode/
[osx-gcc-installer]: https://github.com/kennethreitz/osx-gcc-installer#readme

To fetch the source code from the main Google Code repository,
you'll need a [Mercurial][] client.

To run the tests, you'll need [NUnit][] version 2.5.10 or higher. (The version
that comes with stable builds of Mono at the time of this writing doesn't
support everything used by the unit tests of Noda Time.) Version 2.6.1 is
recommended on non-Windows platforms due to an [NUnit bug][nunit-993247] that
causes tests to fail with a "Too many open files" exception when running some
of the larger suites.

[nunit-993247]: https://bugs.launchpad.net/nunitv2/+bug/993247
  "NUnit Bug #993247: Tests fail with IOException: Too many open files"

### Fetching and building

To fetch the source code, just clone the Google Code repository:

    $ hg clone https://code.google.com/p/noda-time/

Building is performed with `make`, using the included Makefile. (If you don't
have a working `make`, you can also run `xbuild` by hand; see `Makefile` for
the commands you'll need to run.)

    $ cd noda-time
    $ make

This will build all the Noda Time main projects. The main assembly will be
written to `src/NodaTime/bin/Debug/NodaTime.dll`; this is all you need to use
Noda Time itself.

Other build targets are also available; again, see `Makefile` for documentation.
In particular, to build and run the tests, run:

    $ make check

to use the default (probably Mono-supplied) version of NUnit to run the tests,
or something like the following to override the location of the NUnit test
runner:

    $ make check NUNIT='mono ../NUnit-2.6.1/bin/nunit-console.exe'
