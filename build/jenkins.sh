#!/bin/bash

# Check the environment has been set properly
if [ -z "$DOTNET" -o -z "$WEBSITE" ]
then
  echo "Ensure that the DOTNET and WEBSITE environment variables are set"
  exit 1
fi


cd `dirname $0`/..

# Main build and test
$DOTNET restore
$DOTNET build src/NodaTime{,.Testing,.Test,.Serialization.JsonNet,.Test,.TzdbCompiler,.TzdbCompiler.Test}
dotnet run -p src/NodaTime.Serialization.Test
dotnet run -p src/NodaTime.Test -- "--where=cat != Slow && cat != Overflow"
dotnet run -p src/NodaTime.TzdbCompiler.Test

# Temporarily go back to using batch files...
pushd build
cmd //c buildweb.bat $WEBSITE
popd
