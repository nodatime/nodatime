#!/bin/bash

set -e

if [[ "$1" = "" ]]
then
  echo "Usage: buildweb.sh output-directory"
  echo "e.g. buildweb c:\\users\\jon\\NodaTime\\nodatime.org"
  echo "It is expected that the output directory already exists and is"
  echo "set up for git..."
  exit 1
fi

WEB_DIR="$1"

# Build the API docs with docfx
./buildapidocs.sh
rm -rf ../src/NodaTime.Web/docfx
mv tmp/docfx/_site ../src/NodaTime.Web/docfx

# Build the web site ASP.NET Core
rm -rf ../src/NodaTime.Web/bin/Release
dotnet restore ../src/NodaTime.Web
dotnet publish -c Release ../src/NodaTime.Web

# Retain just the .git directory, but nuke the rest from orbit.
rm -rf tmp/old_nodatime.org
mv $WEB_DIR tmp/old_nodatime.org
mkdir $WEB_DIR
mv tmp/old_nodatime.org/.git $WEB_DIR

# Copy the new site into place
cp -r ../src/NodaTime.Web/bin/Release/netcoreapp1.0/publish/* $WEB_DIR
