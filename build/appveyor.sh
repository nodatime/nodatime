#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

dotnet --info

dotnet build -c Release src/NodaTime-All.sln

# Run the tests under dotCover
build/coverage.sh

dotnet test -c Release src/NodaTime.Test --filter=TestCategory!=Slow

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test

dotnet build src/NodaTime-Web.sln
dotnet test src/NodaTime.Web.Test
