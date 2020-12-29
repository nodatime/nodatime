#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

set -e

export ContinuousIntegrationBuild=true

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

dotnet --info

dotnet build -c Release src/NodaTime.sln

dotnet test -c Release src/NodaTime.Test -s src/NodaTime.Test/NoSlowTests.runsettings
dotnet test -c Release src/NodaTime.Demo

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test

# Run the tests under dotCover. (This is after the non-coverage tests,
# so that if there are any test failures we get those sooner.)
build/coverage.sh
