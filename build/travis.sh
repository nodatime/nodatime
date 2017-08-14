#!/bin/bash

set -e

dotnet restore src/NodaTime-All.sln
dotnet restore src/NodaTime-Web.sln
dotnet build src/NodaTime/*.csproj -f netstandard1.3
dotnet build src/NodaTime.Testing/*.csproj -f netstandard1.3
dotnet build src/NodaTime.Benchmarks/*.csproj -f netcoreapp1.1
dotnet build src/NodaTime.Demo/*.csproj -f netcoreapp1.0
dotnet build src/NodaTime.Test/*.csproj -f netcoreapp1.0
dotnet run -p src/NodaTime.Test/*.csproj -f netcoreapp1.0 -- --where=cat!=Slow
dotnet build src/NodaTime.Web/*.csproj -f netcoreapp1.0
dotnet build src/NodaTime.Web.Test/*.csproj -f netcoreapp1.0
dotnet run -p src/NodaTime.Web.Test/*.csproj -f netcoreapp1.0
