#!/bin/bash

set -e

dotnet build src/NodaTime -f netstandard1.3
dotnet build src/NodaTime.Testing -f netstandard1.3
dotnet build src/NodaTime.Benchmarks -f netcoreapp1.1
dotnet build src/NodaTime.Demo -f netcoreapp1.0
dotnet build src/NodaTime.Test -f netcoreapp1.0
dotnet run -p src/NodaTime.Test -f netcoreapp1.0 -- --where=cat!=Slow
dotnet run -p src/NodaTime.Test -f netcoreapp2.0 -- --where=cat!=Slow
dotnet build src/NodaTime.Web -f netcoreapp1.0
dotnet build src/NodaTime.Web.Test -f netcoreapp1.0
dotnet run -p src/NodaTime.Web.Test -f netcoreapp1.0
