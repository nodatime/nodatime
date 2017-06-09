#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

cd $(dirname $0)/..

dotnet restore src/NodaTime-All.sln
dotnet build -c Release src/NodaTime-All.sln

if [ -n "$COVERALLS_REPO_TOKEN" ]
then
  build/coverage.sh
  packages/coveralls.net.0.7.0/tools/csmacnz.Coveralls.exe --opencover -i coverage.xml --useRelativePaths
else
  # Just run tests instead...
  dotnet run -c Release -f net451 -p src/NodaTime.Test/*.csproj -- --where=cat!=Slow
fi
