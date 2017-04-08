@Title="Release process"

This document describes how Noda Time is released. It is intended to be used as
a checklist by the person doing a release.

## Prerequisites

- Visual Studio 2015
- Sandcastle and Sandcastle Help File Builder
- [Jekyll][] and [redcarpet][] (following the Jekyll Windows installation instructions)
- NuGet command-line tool
- Access to the Noda Time release private key (to produce a strong-named
  assembly) and the nuget.org API key
- Access to the (git) repository for the nodatime.org web site

[Jekyll]: https://jekyllrb.com/docs/installation/
[redcarpet]: https://rubygems.org/gems/redcarpet

Note that we should not build releases on Mono at present, since the resulting
IL triggers a bug in the .NET 4 64-bit CLR (see the
[building and testing](building) section in the developer guide).

## When to release

When everybody's happy, there are no issues outstanding for the milestone, and
all the tests pass.

Search the issue tracker for open issues with the right milestone (e.g.
`is:open is:issue milestone:1.4.0`).

Update to the candidate revision (probably HEAD) of the correct branch (e.g.
`1.0.x`) and [build and run all the tests](building) as normal. The build
and test steps should pass on Visual Studio 2015 and at least one supported
OS/version combination for Mono (i.e. 'Mono 2.10.9 on Linux').

## Update the embedded tzdb

If necessary, update the version of tzdb on the branch to the latest current
version, following the instructions in the
["Updating the time zone database"][tzdb] section in the user guide.

[tzdb]: /userguide/tzdb

## Pre-release branching

Before building the first release with a given minor version number (i.e. 1.0.0,
1.1.0, or more typically, a -beta or -rc version of the same), we'll need to
branch. New user-visible functionality is introduced with a new minor version
number (per the [Semantic versioning](http://semver.org/) spec).

The branching model used by Noda Time is the Subversion-style backport model.
In brief:

- feature development is carried out on the master branch
- named branches (called '1.0.x', '1.1.x', etc) are used for release lines
- fixes are typically backported to the earlier branches where necessary
  (typically using `git cherry-pick`).

Note the difference between the format of names used for tags ('1.0.0-beta2',
'1.0.0') and those used for branches ('1.0.x'). Git will allow both to be
used as "tree-ish" values, so it is important that they do not collide.

### Branch-specific changes

The only change that needs to be made to the branch after creation is to
remove the `<Preliminary/>` tag from the Sandcastle project file; see e.g.
[issue 102].

### Post-branch updates on the master branch

If this release required the creation of a new branch, then the following files
on the `master` branch should be updated to bump (at least) the minor version
number (and `NodaTime.Testing` / `NodaTime.Serialization.JsonNet` dependency
version), per the following scheme:

Suppose we were building version 1.2.3-beta4, then:

> - In the NuGet package specs, `<version>` should contain the version number
  ('1.2.3-beta4'). The `<dependency>` element in `NodaTime.Testing.nuspec`
  should reference the _initial release_ for the major/minor version
  (i.e. '1.2.0') for stable versions, or the full version for
  pre-release versions.
> - In the AssemblyInfo files, `AssemblyVersion` should be set to just the
  major/minor version ('1.2'). `AssemblyFileVersion` should be set to the
  major, minor, and patch version ('1.2.3'), and `AssemblyInformationalVersion`
  should be set to the version number ('1.2.3-beta4'). (See [this Stack
  Overflow post][assemblyversion] for more information about how these are
  used.)

[assemblyversion]: https://stackoverflow.com/a/65062

Update the version number by building the tools solution and then running the `SetVersion` tool:

```bat
msbuild build\Tools.sln
build\SetVersion\bin\Debug\SetVersion.exe 1.1.0-dev
```

The version number string should be of the form `1.1.0-dev`.

## Making the release

Switch to the correct branch (e.g. `1.0.x`).

Update the version number according to the scheme above by building the tools
solution and then running the `SetVersion` tool:

```bat
msbuild tools\Tools.sln
build\SetVersion\bin\Debug\SetVersion.exe 1.2.3-beta4
```

> This will update the following AssemblyInfo files and NuGet package specs to include the
version number you are building:

> - `src/NodaTime/Properties/AssemblyInfo.cs`
> - `src/NodaTime.Serialization.JsonNet/Properties/AssemblyInfo.cs`
> - `src/NodaTime.Testing/Properties/AssemblyInfo.cs`
> - `src/NodaTime/NodaTime.nuspec`
> - `src/NodaTime.Serialization.JsonNet/NodaTime.Serialization.JsonNet.nuspec`
> - `src/NodaTime.Testing/NodaTime.Testing.nuspec`

Add the current date and TZDB version to the version history in
`www/unstable/userguide/versions.md` and regenerate all documentation.
(The branch version of the `www/` directory is only used for the offline
user guide; we leave the documentation in `unstable/` for branches.)

Commit the above, then tag that commit:

```bat
git tag 1.0.0-beta1
```

(If the tag has already been applied, use `-f` to force it.)

Switch back to the master branch and update the version history in that branch
to match the changes applied to the release branch, then (if this release
changes the latest stable release), update the date and version number of the
latest stable version in `www/_config.yml`, and (if that new stable release is
not a patch release), also update the `/api` and `/userguide` redirects in
`src/NodaTime.Web/web.config`.

Push these changes to GitHub, ensuring that you push the tags as well as any changes.
(Use `--tags` from the command line.)

## Building the release artifacts

First, *create a new clone for building the release*. Doing this avoids the
need to worry about ignored files and local changes making their way into
the archives.

Copy the Noda Time private key into the top-level directory.

Use the `build\buildrelease.bat` batch file to build all the release artifacts:

```bat
cd build
buildrelease 1.0.0-beta1
```

This will create:

- A zip file of pristine sources (e.g. `NodaTime-1.0.0-beta1-src.zip`)
- A zip file for binary distribution (e.g. `NodaTime-1.0.0-beta1.zip`)
- NuGet packages (in the `nuget` directory)

(See `buildrelease.bat` for details.)

## Publishing the artifacts

Upload the source and release zipfiles to the project website, under
the `downloads` directory. To do this, pull the
[`nodatime.org` repository](https://github.com/nodatime/nodatime.org),
update the `downloads` directory accordingly, commit and then push back
up to GitHub. The web site will update automatically.

Edit the `downloads/index.html` file to include the new downloads,
including SHA-256 hash.

If this is a stable release, update the version number and the links on the
front page to point to the new downloads.

Upload the three NuGet packages to nuget.org:

```bat
nuget push NodaTime.1.0.0-beta1.nupkg
nuget push NodaTime.Testing.1.0.0-beta1.nupkg
nuget push NodaTime.Serialization.JsonNet.1.0.0-beta1.nupkg
```

## Updating the generated API documentation

Copy the generated API documentation from `docs/api` to the corresponding
`/$BRANCH/api` directory in the `nodatime.org` repository.

## Announcing the release

Post to the mailing list, blog, etc.

## Post-release updates on the master branch

If this release was a stable release, then the generated XML documentation
should be copied to `data/versionxml/` on the master branch.

If this release required the creation of a new branch, then the following
updates need to be made to the master branch for the website:

- the user guide source in `www/unstable/userguide` should be copied to
  `www/$BRANCH/userguide` (except for `versions.md`, which should come from the
  branch copy)
- the Jekyll/Liquid templates (`www/_layouts/foundation.html`) should be
  adjusted to add the new branch for the "API" and "User Guide" dropdowns
