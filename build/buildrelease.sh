#!/bin/bash

set -e

cd $(dirname $0)

if [[ $# -ne 1 ]]; then
  echo 'Usage: buildrelease.sh version'
  echo 'e.g. buildrelease.sh 2.0.0'
  echo 'or buildrelease.sh 2.0.0-rc1'
  echo 'It is expected that a git tag will already exist'
  exit 1
fi

# Make sure the packages end up with suitable embedded paths
export ContinuousIntegrationBuild=true

declare -r VERSION=$1
declare -r SUFFIX=$(echo $VERSION | cut -s -d- -f2)
declare -r BUILD_FLAG=${SUFFIX:+--version-suffix ${SUFFIX}}
declare -r RESTORE_FLAG=${SUFFIX:+-p:VersionSuffix=${SUFFIX}}
declare -r OUTPUT=artifacts

rm -rf releasebuild
git clone https://github.com/nodatime/nodatime.git releasebuild -c core.autocrlf=input
cd releasebuild
git checkout "$VERSION"

# See https://github.com/nodatime/nodatime/issues/713
# and https://github.com/NuGet/Home/issues/3953
# ... but note that from bash, /p has to be -p
dotnet restore $RESTORE_FLAG src/NodaTime
dotnet restore $RESTORE_FLAG src/NodaTime.Testing
dotnet restore $RESTORE_FLAG src/NodaTime.Test

dotnet build -c Release $BUILD_FLAG -p:SourceLinkCreate=true src/NodaTime
dotnet build -c Release $BUILD_FLAG -p:SourceLinkCreate=true src/NodaTime.Testing
dotnet build -c Release $BUILD_FLAG -p:SourceLinkCreate=true src/NodaTime.Test

# Even run the slow tests before a release...
dotnet test -c Release src/NodaTime.Test

mkdir $OUTPUT

dotnet pack --no-build -c Release $BUILD_FLAG src/NodaTime
dotnet pack --no-build -c Release $BUILD_FLAG src/NodaTime.Testing
cp src/NodaTime/bin/Release/*.nupkg $OUTPUT
cp src/NodaTime.Testing/bin/Release/*.nupkg $OUTPUT
git archive $VERSION -o $OUTPUT/NodaTime-$VERSION-src.zip --prefix=NodaTime-$VERSION-src/

mkdir NodaTime-$VERSION
cp AUTHORS.txt NodaTime-$VERSION
cp LICENSE.txt NodaTime-$VERSION
cp NOTICE.txt NodaTime-$VERSION
cp 'NodaTime Release Public Key.snk' NodaTime-$VERSION
cp build/zip-readme.txt NodaTime-$VERSION/readme.txt
mkdir NodaTime-$VERSION/Portable

# Doc files
cp src/NodaTime/bin/Release/netstandard2.0/NodaTime.xml NodaTime-$VERSION
cp src/NodaTime.Testing/bin/Release/netstandard2.0/NodaTime.Testing.xml NodaTime-$VERSION

# Assemblies
cp src/NodaTime/bin/Release/netstandard2.0/NodaTime.dll NodaTime-$VERSION
cp src/NodaTime.Testing/bin/Release/netstandard2.0/NodaTime.Testing.dll NodaTime-$VERSION

# PDBs
cp src/NodaTime/bin/Release/netstandard2.0/NodaTime.pdb NodaTime-$VERSION
cp src/NodaTime.Testing/bin/Release/netstandard2.0/NodaTime.Testing.pdb NodaTime-$VERSION

declare -r BUILD_DATE=$(git show -s --format=%cI)

# see https://wiki.debian.org/ReproducibleBuilds/TimestampsInZip.
find NodaTime-$VERSION -print0 | \
  xargs -0r touch --no-dereference --date="${BUILD_DATE}"
# Assumption: -X on Windows zip is equivalent to --no-extra
TZ=UTC zip -r -9 -X --latest-time $OUTPUT/NodaTime-$VERSION.zip NodaTime-$VERSION
