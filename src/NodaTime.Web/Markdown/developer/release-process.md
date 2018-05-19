@Title="Release process"

This document describes how Noda Time is released. It is intended to be used as
a checklist by the person doing a release. It only covers doing a
new major/minor release; patch releases are generally just a matter
of tagging the right branch and running `buildrelease.sh`, then
pushing the results as shown below.

## Prerequisites

- Visual Studio 2017
- .NET Core SDKs (version will change over time; check global.json)
- NuGet command-line tool
- Appropriate access to Google Cloud Storage and NuGet
- git for Windows
- Bash that comes with git for Windows

## When to release

When everybody's happy, there are no issues outstanding for the milestone, and
all the tests pass.

Search the issue tracker for open issues with the right milestone (e.g.
`is:open is:issue milestone:1.4.0`).

## Preparing

- In GitHub, create branch `2.3.x` from master
- Protect the branch (in GitHub repository settings)
- Create a release in GitHub, with a new tag `2.3.0` against the new branch

## Building

This is performed locally, in bash. We currently use the bash that
comes with Git for Windows. The release scripts may work in other
environments too.

- Optionally fetch the new branch (it doesn't matter too much)
- In the build directory, run `./buildrelease.sh 2.3.0` and wait.
  This will clone the repo for the specific tag and build
  everything required. It takes a little while as even the slow
  tests are run.
- The build results will be in `releasebuild/artifacts`. Switch to
  that directory.
- Upload the zip files to Google Cloud Storage, e.g. with

      gsutil.cmd cp *.zip gs://nodatime/releases

- Push the NuGet packages, e.g. with  
  
      for i in *.nupkg; do nuget push -source https://api.nuget.org/v3/index.json -apikey API_KEY $i; done

- Hash the storage files. In the `build` directory, run

      dotnet run -p HashStorageFiles
        
## Post-release

Make changes in the master branch

- Edit the project files for NodaTime and NodaTime.Testing with the
  expected next version number
- Edit the version history to record the release
- Rename the `unstable` directory in NodaTime.Web/Markdown to `2.3.x`
- Edit `2.3.x/index.json` to specify the name `2.3.x`
- Create a new `unstable` directory and copy index.json from `2.3.x`
- Edit `unstable/index.json` to have a name of `unstable` and a
  parent of `2.3.x`
- Edit `Startup.cs`:
  - Add a `MapRoute` call for the 2.3.x user guide
  - Edit the last `AddRedirect` call to refer to 2.3.x
  - Change the `anyVersion` regex to include 2.3.x
  - Edit `Views/Shared/_Layout.cshtml` to include 2.3.x in the menu
    and make it "current" while 2.2.x becomes "Old" (both user guide and API)
- Change the build scripts
  - Edit `buildhistory.sh` (this is pretty involved)
  - Edit `buildapidocs.sh`
  - Copy `build/docfx/docfx-2.2.x.json` to `docfx-2.3.x.json`
  - Edit `docfx-unstable.json` to include 2.3.x in `content` and `overwrite`
  - Copy `tzdbupdate/update-2.2.sh` to `update-2.3.sh` and edit it accordingly
  - Edit `tzdbupdate/update-all.sh` to call the new script
- Rebuild history
  - Run `build/buildhistory.sh`
  - Check the results, and push them as indicated by the script

Create a pull request with all these changes in. Review carefully,
and merge. The web site will then automatically be updated.
