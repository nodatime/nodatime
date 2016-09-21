#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

set -e

# cd to repository root
cd $(dirname $0)/..

dotnet restore src
dotnet build -c Release src/NodaTime.Test
dotnet test -c Release -f net451 src/NodaTime.Test --where=cat!=Slow

if [ -n "$COVERALLS_REPO_TOKEN" ]
then
  nuget install -OutputDirectory packages -Version 4.6.519 OpenCover
  nuget install -OutputDirectory packages -Version 0.7.0 coveralls.net
  packages/OpenCover.4.6.519/tools/OpenCover.Console.exe \
    -register:user \
    -oldStyle \
    -target:"c:\Program Files\dotnet\dotnet.exe" \
    -targetargs:"test -f net451 src/NodaTime.Test -where=cat!=Slow" \
    -output:coverage.xml \
    -filter:"+[NodaTime]*" \
    -searchdirs:NodaTime/bin/Release/net451/win7-x64

  packages/coveralls.net.0.7.0/tools/csmacnz.Coveralls.exe --opencover -i coverage.xml --useRelativePaths
fi
