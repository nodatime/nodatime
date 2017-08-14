#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

cd $(dirname $0)/..

dotnet restore src/NodaTime-All.sln
dotnet build -c Release src/NodaTime-All.sln
dotnet restore src/NodaTime-Web.sln
dotnet build src/NodaTime-Web.sln

# TODO: Reinstate coverage

dotnet run -c Release -f net451 -p src/NodaTime.Test/*.csproj -- --where=cat!=Slow
dotnet run -f netcoreapp1.0 -p src/NodaTime.Web.Test/*.csproj
