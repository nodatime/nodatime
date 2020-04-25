#!/bin/bash

set -e

export ContinuousIntegrationBuild=true

dotnet --info

dotnet build -c Release src/NodaTime.sln
dotnet test -c Release src/NodaTime.Test -s src/NodaTime.Test/NoSlowTests.runsettings
dotnet test -c Release src/NodaTime.Demo

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test
