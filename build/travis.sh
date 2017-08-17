#!/bin/bash

set -e

dotnet build -c Release src/NodaTime -f netstandard1.3
dotnet build -c Release src/NodaTime.Testing -f netstandard1.3
dotnet build -c Release src/NodaTime.Benchmarks -f netcoreapp1.1
dotnet build -c Release src/NodaTime.Demo -f netcoreapp1.0
dotnet build -c Release src/NodaTime.Test -f netcoreapp1.0
dotnet run -c Release -p src/NodaTime.Test -f netcoreapp1.0 -- --test=NodaTime.Test.TimeZones.TzdbDateTimeZoneSourceTest.GuessZoneIdByTransitionsUncached
dotnet run -c Release -p src/NodaTime.Test -f netcoreapp2.0 -- --test=NodaTime.Test.TimeZones.TzdbDateTimeZoneSourceTest.GuessZoneIdByTransitionsUncached
dotnet build src/NodaTime.Web -f netcoreapp1.0
dotnet build src/NodaTime.Web.Test -f netcoreapp1.0
dotnet run -p src/NodaTime.Web.Test -f netcoreapp1.0
