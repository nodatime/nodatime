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

dotnet run --no-build -c Release -p src/NodaTime.Test -- --where=cat!=Slow

dotnet build src/NodaTime-Web.sln
dotnet run -p src/NodaTime.Web.Test

