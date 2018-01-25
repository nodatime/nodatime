#!/bin/bash

set -e

if [[ "$1" = "" ]]
then
  echo "Usage: buildweb.sh output-directory [--skip-api]"
  echo "e.g. buildweb.sh c:\\users\\jon\\NodaTime\\nodatime.org"
  echo "It is expected that the output directory already exists and is"
  echo "set up for git. If --skip-api is set, it is assumed the API docs already exist."
  exit 1
fi

WEB_DIR="$1"

# Build the API docs with docfx
if [[ "$2" != "--skip-api" ]]
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

# Retain just the .git directory, but nuke the rest from orbit.
rm -rf tmp/old_nodatime.org
mv $WEB_DIR tmp/old_nodatime.org
mkdir $WEB_DIR
mv tmp/old_nodatime.org/.git $WEB_DIR

# Copy the new site into place
cp -r ../src/NodaTime.Web/bin/Release/netcoreapp2.0/publish/* $WEB_DIR

# Run a smoke test to check it still works
(cd $WEB_DIR; dotnet NodaTime.Web.dll --smoke-test)
