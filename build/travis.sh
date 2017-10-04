#!/bin/bash

set -e

dotnet build -c Release src/NodaTime -f netstandard1.3
dotnet build -c Release src/NodaTime.Testing -f netstandard1.3
dotnet build -c Release src/NodaTime.Benchmarks -f netcoreapp1.1
dotnet run -c Release -p src/NodaTime.Test -f netcoreapp1.0 -- --where=cat!=Slow
dotnet run -c Release -p src/NodaTime.Test -f netcoreapp2.0 -- --where=cat!=Slow

dotnet build src/NodaTime.Web -f netcoreapp2.0
dotnet run -p src/NodaTime.Web.Test -f netcoreapp2.0
