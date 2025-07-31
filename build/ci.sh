#!/bin/bash

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

export ContinuousIntegrationBuild=true
dotnet build -c Release src/NodaTime.sln
dotnet test -c Release -f net8.0 src/NodaTime.Test
dotnet test -c Release -f net8.0 src/NodaTime.Demo

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release -f net8.0 src/NodaTime.TzdbCompiler.Test

# Publish the AOT compatibility app as an additional step to
# find any AOT-problematic code.
dotnet publish src/NodaTime.AotCompatibilityTestApp

# Pack the production projects, for compatibility testing.
dotnet pack src/NodaTime
dotnet pack src/NodaTime.Testing
