#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

cd $(dirname $0)/..

if [ -n "$COVERALLS_REPO_TOKEN" ]
then
  build/coverage.sh
  packages/coveralls.net.0.7.0/tools/csmacnz.Coveralls.exe --opencover -i coverage.xml --useRelativePaths
else
  # Just do the build and test instead...
  dotnet restore src
  dotnet build -c Release src/NodaTime.Test
  dotnet test -c Release -f net451 src/NodaTime.Test --where=cat!=Slow
fi
