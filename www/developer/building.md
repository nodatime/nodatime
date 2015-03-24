---
layout: developer
title: Building and testing
weight: 100
---

## Visual Studio (Windows)

TODO: As of 2.0, the following is no longer correct: _building_ Noda Time
2.0 requires a C# 6 compiler.

Noda Time is mostly developed on Visual Studio 2010 and Visual Studio 2012.
All versions of Visual Studio 2010 and 2012 which support C#, including Express editions,
should be able to build Noda Time. If you're using Visual Studio Express, you *may* have
problems building the PCL version; it depends on other aspects of your computer. Visual
Studio Express itself doesn't have full support for portable class libraries, so while
it's important to us that developers with only Visual Studio Express can work on the
desktop build, we make no guarantees about the PCL build.
(Please contact the mailing list if you have problems not covered here.) 

You'll also need the [.NET Framework SDK][dotnetsdk]. This library supports
versions 3.5, 4 and 4.5 of the Framework. Install the appropriate
version or versions as defined by your needs.

[dotnetsdk]: http://msdn.microsoft.com/en-us/netframework/aa569263.aspx

To fetch the source code from the main GitHub repository, you'll need a
[git][] client. A good alternative for Microsoft Windows users is
[TortoiseGit][] which installs shell extensions so that git can be used
from the Windows Explorer.

[git]: http://git-scm.com/
[TortoiseGit]: https://tortoisegit.org/

To run the tests, you'll need [NUnit][] version 2.5.10 or higher.

[NUnit]: http://nunit.org/index.php?p=download

Unfortunately though we target .NET 3.5 for *running* Noda Time on the desktop, the unit test
library targets .NET 4. This is mostly so that we can easily run the tests against the PCL
build. We would like to run the desktop build tests against .NET 3.5 and the PCL build tests
against .NET 4, but that sort of subtlety seems to cause problems with the ability of
test runners to autodetect which version they should load. We don't use any .NET 4 features
in our tests though; if you hack at the project file to make it only target .NET 3.5, you should
be able to test the desktop build that way, if you really need to.

### Fetching and building

To fetch the source code, just clone the GitHub repository:

```bat
> git clone https://github.com/nodatime/nodatime.git
```

To build everything under Visual Studio, simply open the `src\NodaTime-All.sln` file.
To build with just the SDK and msbuild, run

```bat
> msbuild src\NodaTime-All.sln /property:Configuration=Debug
```

Execute the tests using your favourite NUnit test runner. For example:

```bat
> nunit-console src\NodaTime.Test\bin\Debug\NodaTime.Test.dll
```

(Include the other test DLLs should you wish to, of course.)

## Mono

_Building_ Noda Time 2.0 requires a C# 6 compiler.

As of March 2015, no released version of Mono supports C# 6, though
built-from-source versions of what will become Mono 4.0.0 do.

While not intended to be a complete guide to installing Mono from source, on
Linux, you probably want to do something approximately like the following:

```sh
$ sudo apt-get install autoconf automake libtool-bin  # etc

$ PREFIX=$HOME/mono/4.0.0  # or wherever
$ git clone git@github.com:mono/mono.git mono-git
$ cd mono-git/
$ git checkout mono-4.0.0-branch
$ git submodule init
$ git submodule update
$ ./autogen.sh --prefix=$PREFIX
$ make get-monolite-latest
$ make EXTERNAL_MCS=${PWD}/mcs/class/lib/monolite/basic.exe
$ make install
$ export PATH=$PREFIX/bin:$PATH
$ mono --version | head -n1
Mono JIT compiler version 4.0.0 (mono-4.0.0-branch/b7bcd23 Tue 24 Mar 16:03:12 GMT 2015)
$ mozroots --import --sync  # otherwise NuGet will fail with SSL errors later
Mozilla Roots Importer - version 4.0.0.0
Download and import trusted root certificates from Mozilla's MXR.
Copyright 2002, 2003 Motus Technologies. Copyright 2004-2008 Novell. BSD
licensed.

Downloading from
'http://mxr.mozilla.org/seamonkey/source/security/nss/lib/ckfw/builtins/certdata.txt?raw=1'...
Importing certificates into user store...
140 new root certificates were added to your trust store.
Import process completed.
```

(More generally, see the [Mono documentation][mono-git] for more details
about building from source on Linux.)
[mono-git]: http://www.mono-project.com/docs/compiling-mono/linux/#building-mono-from-a-git-source-code-checkout
  "Compiling Mono on Linux: Building Mono From a Git Source Code Checkout"

### Noda Time 1.x

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

### Fetching dependencies

Building Noda Time requires a small number of third-party packages.  These
can be most easily obtained via [NuGet].

[NuGet]: https://www.nuget.org/

To fetch NuGet.exe locally, download and run (using `mono`) the
(auto-updating) [NuGet.exe command-line
tool](http://nuget.codeplex.com/releases/view/58939).

You can then fetch the NuGet packages locally (into `src/packages/`):

```sh
$ make fetch-packages NUGET=/path/to/NuGet.exe
```

(`NUGET=` can be omitted if NuGet.exe is in the current directory.)

### Fetching and building

To fetch the source code from the main GitHub repository, you'll need a
[git][] client.

```sh
$ git clone https://github.com/nodatime/nodatime.git
```

Building is performed with `make`, using the included Makefile. (If you don't
have a working `make`, you can also run `xbuild` by hand; see `Makefile` for
the commands you'll need to run.)

```sh
$ cd nodatime
$ make
```

This will build all the Noda Time main projects. The main assembly will be
written to `src/NodaTime/bin/Debug/NodaTime.dll`; this is all you need to use
Noda Time itself.

Other build targets are also available; again, see `Makefile` for documentation.

To run the tests, you'll need [NUnit][] version 2.5.10 or higher. (The
version that comes with Mono doesn't support everything used by the unit
tests of Noda Time.) Version 2.6.1 or higher is recommended on non-Windows
platforms due to an [NUnit bug][nunit-993247] that causes tests to fail with
a "Too many open files" exception when running some of the larger suites.

[nunit-993247]: https://bugs.launchpad.net/nunitv2/+bug/993247
  "NUnit Bug #993247: Tests fail with IOException: Too many open files"

To build and run the tests, run:

```sh
$ make check
```

to use the default (from `PATH`) version of NUnit, or something like the
following to override the location of the NUnit test runner:

```sh
$ make check NUNIT='mono ../NUnit-2.6.1/bin/nunit-console.exe'
```

### Source layout

All the source code is under the `src` directory. There are multiple projects:

- NodaTime: The main library to be distributed
- NodaTime.Benchmarks: Benchmarks run regularly to check the performance
- NodaTime.CheckTimeZones: Tool to verify that Joda Time and Noda Time interpret TZDB data in the same way
- NodaTime.Demo: Demonstration code written as unit tests. Interesting [Stack Overflow](http://stackoverflow.com) questions can lead to code in this project, for example.
- NodaTime.Serialization.JsonNet: Library to enable [Json.NET](http://json.net) serialization of NodaTime types.
- NodaTime.Serialization.Test: Tests for all serialization assemblies, under the assumption that at some point we'll support more than just Json.NET.
- NodaTime.Test: Tests for the main library
- NodaTime.Testing: Library to help users test code which depends on Noda Time. Also used within our own tests.
- NodaTime.Tools.SetVersion: Tool to set version numbers on appropriate assemblies as part of the release process
- NodaTime.TzdbCompiler: Tool to take a TZDB database and convert it into the NodaTime internal format
- NodaTime.TzdbCompiler.Test: Tests for NodaTime.TzdbCompiler

The documentation is in the `www` directory with the rest of the website: `www/developer` for the developer guide, and `www/unstable/userguide` for the latest user guide.
Additionally the `www` directory contains the source for the documentation, and `JodaDump` contains Java code to be used with NodaTime.CheckTimeZones.


There are four solutions, containing a variety of projects depending on the task at hand. The aim is to have everything you immediately need in one
place, without too many distractions.

- NodaTime-All.sln: Contains all .NET projects, but not the documentation source. Useful for continuous integration and refactoring.
- NodaTime-Core.sln: Contains NodaTime, NodaTime.Benchmarks, NodaTime.Serialization.JsonNet and NodaTime.Serialization.Test, NodaTime.Test, NodaTime.Testing.
 This is the "day to day development" solution, containing everything that's shipped and the primary tests for those projects. When we're updating NodaTime.TzdbCompiler,
 that project and its tests may temporarily live in this solution.
- NodaTime-Documentation.sln: Contains the documentation sources, NodaTime, NodaTime.Testing.SerializationJsonNet and NodaTime.Testing. Useful when writing documentation.
- NodaTime-Tools.sln: Contains NodaTime (so that project references work), NodaTime.CheckTimeZones, NodaTime.Tools.SetVersion, NodaTime.TzdbCompiler
 and NodaTime.TzdbCompiler.Test. Aimed at times when we need to change our supporting toolset.
