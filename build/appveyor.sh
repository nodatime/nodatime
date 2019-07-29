#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

dotnet --info

sed -i -e "s/@VERSION@/${APPVEYOR_BUILD_VERSION}/g" src/NodaTime.TzdbCompiler/NodaTime.TzdbCompiler.nuspec

dotnet build -c Release src/NodaTime.sln

dotnet test -c Release src/NodaTime.Test --filter=TestCategory!=Slow

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test

dotnet pack src/NodaTime.TzdbCompiler -c Release

nuget push src/NodaTime.TzdbCompiler/bin/Release/*.nupkg 5tVFkd4fBBLsizObH7FzrUCdQXXXVwCXgARAlILFFLmfP2xsah4AfZPleCjL7laI -Source https://replicon.myget.org/F/developer/auth/680abe10-0c9b-4b29-b466-d6257b98118e/api/v2

# Run the tests under dotCover. (This is after the non-coverage tests,
# so that if there are any test failures we get those sooner.)
build/coverage.sh
