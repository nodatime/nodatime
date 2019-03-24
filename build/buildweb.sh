#!/bin/bash

set -e

# Note: use --skip-api to skip building API docs

# Disable msbuild node reuse, in an attempt to stabilize the build.
# The bundler/minimizer seems to have problems which *may* be related
# to node reuse.
export MSBUILDDISABLENODEREUSE=1

# Build the API docs with docfx
if [[ "$1" != "--skip-api" ]]
then
  ./buildapidocs.sh
  rm -rf ../src/NodaTime.Web/docfx
  cp -r tmp/docfx/_site ../src/NodaTime.Web/docfx
fi

# Build the web site ASP.NET Core
rm -rf ../src/NodaTime.Web/bin/Release
# Make sure minification happens before publish...
dotnet build -c Release ../src/NodaTime.Web
dotnet publish -c Release ../src/NodaTime.Web

# Fix up blazor.config to work in Unix
# (Blazor is currently disabled.)
# sed -i 's/\\/\//g' $WEB_DIR/NodaTime.Web.Blazor.blazor.config

# Run a smoke test to check it still works
dotnet test ../src/NodaTime.Web.SmokeTest
