#!/bin/bash

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
cd $ROOT

export ContinuousIntegrationBuild=true
dotnet build -c Release src/NodaTime.sln
dotnet test -c Release src/NodaTime.Test
dotnet test -c Release src/NodaTime.Demo

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test
