#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

dotnet --info

dotnet build -c Release src/NodaTime-All.sln

# Run just the net451 tests under dotCover
build/coverage.sh

dotnet run --no-build -c Release -f netcoreapp1.0 -p src/NodaTime.Test -- --where=cat!=Slow
dotnet run --no-build -c Release -f netcoreapp2.0 -p src/NodaTime.Test -- --where=cat!=Slow

dotnet build src/NodaTime-Web.sln
dotnet run -f netcoreapp2.0 -p src/NodaTime.Web.Test

