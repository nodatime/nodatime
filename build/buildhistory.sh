#!/bin/bash

# This script populates the history directory from scratch.
# The directory is stored as an orphan branch called 'history'
# in the https://github.com/nodatime/nodatime repo.
#
# The aim is that scripts such as buildapi.sh can just
# fetch the branch (with a depth of 1) and use it as-is.
# This script should only need to be rerun when we've made
# significant changes, e.g. releasing a new major or minor
# version, or changing the build scripts.

# Steps:
# 1. Blow away any current history directory
# 2. Checkout the current history branch from github
# 3. Populate
# 4. Commit

# Note that this does *not* push the branch back up to github,
# but after making sure that it all looks okay, that would
# normally be the next step.

# Contents:

# - 1.0.x / 1.1.x / 1.2.x / 1.3.x / 1.4.x
#   - Clone of the nodatime repo
#   - Docfx metadata in "api" directory
# - 2.0.x, 2.1.x, 2.2.x
#   - Clone of the nodatime repo directory + Json.NET src 
#     from the nodatime.serialization repo
#   - Docfx metadata in "api" directory
#   - Docfx snippets for 2.1 onwards
# - packages
#   - nupkg files for each minor version, e.g NodaTime-1.0.x.nupkg

set -e

source docfx_functions.sh
install_docfx

echo "Removing old history directory"
rm -rf history

echo "Cloning current history branch"
git clone https://github.com/nodatime/nodatime.git -q -b history history

cd history

echo "Cleaning current directory"
git rm -rf .
git clean -df

for version in 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x
do
  echo "Cloning $version"
  git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b $version $version
  rm -rf $version/.git
done

# 2.0, which has split repositories
echo "Cloning 2.0.x main repo"
git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b 2.0.x 2.0.x
rm -rf 2.0.x/.git

echo "Cloning 2.1.x main repo"
git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b 2.1.x 2.1.x
rm -rf 2.1.x/.git

echo "Cloning 2.2.x main repo"
git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b 2.2.x 2.2.x
rm -rf 2.2.x/.git

echo "Cloning serialization (for NodaTime.Serialization.JsonNet)"
# Not: not depth 1 as we want to check out specific tags
git clone https://github.com/nodatime/nodatime.serialization.git -q serialization
git -C serialization checkout NodaTime.Serialization.JsonNet-2.0.0
cp -r serialization/src/NodaTime.Serialization.JsonNet 2.0.x/src
cp -r serialization/src/NodaTime.Serialization.JsonNet 2.1.x/src
cp -r serialization/src/NodaTime.Serialization.JsonNet 2.2.x/src
rm -rf serialization

echo "Preparing for docfx of 2.0.x"
cd 2.0.x
dotnet restore src/NodaTime
dotnet restore src/NodaTime.Testing
dotnet restore src/NodaTime.Serialization.JsonNet
cd ..

echo "Preparing for docfx of 2.1.x"
cd 2.1.x
dotnet restore src/NodaTime
dotnet restore src/NodaTime.Demo
dotnet restore src/NodaTime.Testing
dotnet restore src/NodaTime.Serialization.JsonNet
dotnet restore build/SnippetExtractor
cd ..

echo "Preparing for docfx of 2.2.x"
cd 2.2.x
dotnet restore src/NodaTime
dotnet restore src/NodaTime.Demo
dotnet restore src/NodaTime.Testing
dotnet restore src/NodaTime.Serialization.JsonNet
dotnet restore build/SnippetExtractor
cd ..

echo "Fetching nuget packages"
mkdir packages
# NodaTime
wget --quiet -Opackages/NodaTime-1.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.0.0
wget --quiet -Opackages/NodaTime-1.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.1.0
wget --quiet -Opackages/NodaTime-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.2.0
wget --quiet -Opackages/NodaTime-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.3.2
wget --quiet -Opackages/NodaTime-1.4.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.4.0
wget --quiet -Opackages/NodaTime-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/2.0.0
wget --quiet -Opackages/NodaTime-2.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/2.1.0
wget --quiet -Opackages/NodaTime-2.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/2.2.0

# NodaTime.Testing
wget --quiet -Opackages/NodaTime.Testing-1.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.0.0
wget --quiet -Opackages/NodaTime.Testing-1.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.1.0
wget --quiet -Opackages/NodaTime.Testing-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.2.0
wget --quiet -Opackages/NodaTime.Testing-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.3.2
wget --quiet -Opackages/NodaTime.Testing-1.4.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.4.0
wget --quiet -Opackages/NodaTime.Testing-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/2.0.0
wget --quiet -Opackages/NodaTime.Testing-2.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/2.1.0
wget --quiet -Opackages/NodaTime.Testing-2.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/2.2.0

# NodaTime.Serialization.JsonNet
wget --quiet -Opackages/NodaTime.Serialization.JsonNet-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/1.2.0
wget --quiet -Opackages/NodaTime.Serialization.JsonNet-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/1.3.2
wget --quiet -Opackages/NodaTime.Serialization.JsonNet-1.4.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/1.4.0
wget --quiet -Opackages/NodaTime.Serialization.JsonNet-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/2.0.0
# TODO: Fix this grotty hack. It's basically pretending that we have a 2.1 serialization package,
# for the sake of later tools.
cp packages/NodaTime.Serialization.JsonNet-2.0.x.nupkg packages/NodaTime.Serialization.JsonNet-2.1.x.nupkg 
cp packages/NodaTime.Serialization.JsonNet-2.0.x.nupkg packages/NodaTime.Serialization.JsonNet-2.2.x.nupkg 

# Docfx metadata
for version in 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x 2.0.x 2.1.x 2.2.x
do
  echo "Building docfx metadata for $version"
  cp ../docfx/docfx-$version.json $version/docfx.json
  "$DOCFX" metadata $version/docfx.json -f
done

# Snippets
for version in 2.1.x 2.2.x
do
  echo "Generating snippets for $version"
  (cd $version;
   dotnet restore src/NodaTime-all.sln;
   dotnet build src/NodaTime-All.sln;
   mkdir overwrite
   dotnet run -p build/SnippetExtractor/SnippetExtractor.csproj -- src/NodaTime-All.sln NodaTime.Demo overwrite)
done

# We don't need TZDB/CLDR/versionXML, or docs, or
# web site
for version in 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x
do
  rm -rf $version/{data,docs,www}
done

# Remove caches etc - we don't need them, and it's more diff for no reason
find . -name 'obj' | xargs rm -rf

cat > README.md <<"End-of-readme"
The contents of this branch are generated by the `build/buildhistory.sh`
script in the master branch. All commits to this branch should
originate from that script. No hand edits!
End-of-readme

# Just tell Travis not to build this branch. We don't
# care what this branch says about other branches.
cat > .travis.yml<<"End-of-travis"
branches:
  except:
  - history
End-of-travis

git add --all
git commit -m "Regenerated history directory"

cd ..
echo "Done. Check for errors, then push to github"
