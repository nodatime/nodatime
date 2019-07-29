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

dotnet test -c Release src/NodaTime.TzdbCompiler.Test

dotnet pack src/NodaTime.TzdbCompiler -c Release