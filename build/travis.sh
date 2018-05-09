#!/bin/bash

set -e

dotnet build -c Release src/NodaTime
dotnet build -c Release src/NodaTime.Testing
dotnet build -c Release src/NodaTime.Benchmarks
dotnet run -c Release -p src/NodaTime.Test -- --where=cat!=Slow

dotnet build src/NodaTime.Web
dotnet run -p src/NodaTime.Web.Test
