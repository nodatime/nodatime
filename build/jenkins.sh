#!/bin/bash

# Check the environment has been set properly
if [ -z "$DNU" -o -z "$WEBSITE" ]
then
  echo "Ensure that the DNU and WEBSITE environment variables are set"
  exit 1
fi


cd `dirname $0`/..

# Main build and test
$DNU restore
$DNU build src/NodaTime
$DNU build src/NodaTime.Testing
$DNU build src/NodaTime.Test
$DNU build src/NodaTime.Serialization.JsonNet
$DNU build src/NodaTime.Serialization.Test
$DNU build src/NodaTime.TzdbCompiler.Test
dnx -p src/NodaTime.Serialization.Test test
dnx -p src/NodaTime.Test test "--where=cat != Slow && cat != Overflow"
dnx -p src/NodaTime.TzdbCompiler.Test test

# Temporarily go back to using batch files...
pushd build
buildweb.bat $WEBSITE
popd
