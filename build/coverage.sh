#!/bin/bash

set -e

# cd to repository root
cd $(dirname $0)/..

nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 0.7.0 coveralls.net
nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator

dotnet restore src
dotnet build -c Release src/NodaTime.Test

# Note: need to run netcoreapp1.0 build to avoid InvalidProgramException,
# due to RyuJIT (I think). See 
# http://stackoverflow.com/questions/36755337
# This does mean we don't get coverage for types not in .NET Core though.
packages/OpenCover.4.6.519/tools/OpenCover.Console.exe \
  -register:user \
  -oldStyle \
  -target:"c:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test -f netcoreapp1.0 -c Release src/NodaTime.Test -where=cat!=Slow" \
  -output:coverage.xml \
  -filter:"+[NodaTime]NodaTime.*" \
  -searchdirs:NodaTime/bin/Release/net451/win7-x64

packages/ReportGenerator.2.4.5.0/tools/ReportGenerator.exe \
  -reports:coverage.xml \
  -targetdir:coverage \
  -verbosity:Error
