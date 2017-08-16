#!/bin/bash

# Build script run by Appveyor. Might eventually be less
# single-purpose, but let's get coverage going ASAP...

cd $(dirname $0)/..

dotnet --info

dotnet build -c Release src/NodaTime-All.sln

# TODO: Reinstate coverage

dotnet run -c Release -f net451 -p src/NodaTime.Test -- --where=cat!=Slow
dotnet run -c Release -f netcoreapp1.0 -p src/NodaTime.Test -- --where=cat!=Slow
dotnet run -c Release -f netcoreapp2.0 -p src/NodaTime.Test -- --where=cat!=Slow

dotnet build src/NodaTime-Web.sln
dotnet run -f netcoreapp1.0 -p src/NodaTime.Web.Test
