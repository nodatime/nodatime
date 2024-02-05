#!/bin/bash

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

export ContinuousIntegrationBuild=true
dotnet build -c Release src/NodaTime.sln
dotnet test -c Release -f net8.0 src/NodaTime.Test
dotnet test -c Release -f net8.0 src/NodaTime.Demo
dotnet test -c Release -f net6.0 src/NodaTime.Test

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release -f net8.0 src/NodaTime.TzdbCompiler.Test
