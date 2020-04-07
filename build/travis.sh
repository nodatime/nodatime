#!/bin/bash

set -e

dotnet --info
mono --version || true

dotnet build -c Release src/NodaTime.sln
dotnet test -c Release -f "${TARGET_FRAMEWORK}" src/NodaTime.Test -s src/NodaTime.Test/NoSlowTests.runsettings
dotnet test -c Release -f "${TARGET_FRAMEWORK}" src/NodaTime.Demo
dotnet test -c Release -f "${TARGET_FRAMEWORK}" src/NodaTime.TzdbCompiler.Test
