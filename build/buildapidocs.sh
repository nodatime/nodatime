#!/bin/bash

set -e

PREVIOUS_VERSIONS="1.0.x 1.1.x 1.2.x 1.3.x"
echo "Fetching previous versions from source control if necessary"
if [[ ! -d history ]]
then
  mkdir history
  for version in $PREVIOUS_VERSIONS
  do
    echo "Cloning $version"
    git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b $version history/$version
  done
else
  echo Directory for previous versions already exists.
  echo Checking we have all the versions we need...
  for version in $PREVIOUS_VERSIONS
  do
    if [[ ! -d "history/$version" ]]
    then
      echo "Previous version $version has not been cloned."
      echo "Please delete the history directory and rerun."
      exit 1
    fi
  done
  echo Looks good.
fi

rm -rf tmp/docfx
mkdir -p tmp/docfx

for version in $PREVIOUS_VERSIONS; do
  echo "Building metadata for $version"
  rm -rf history/$version/api
  mkdir -p tmp/docfx/obj/$version
  cp docfx/docfx-csproj.json history/$version/docfx.json
  docfx metadata history/$version/docfx.json -f
  cp -r history/$version/api tmp/docfx/obj/$version
  cp docfx/toc.yml tmp/docfx/obj/$version
done

echo "Building metadata for current branch"
# Docfx doesn't support VS2017 csproj files yet. Sigh.
# Also, we want to include the serialization docs (sometimes? unclear)

git clone https://github.com/nodatime/nodatime.serialization.git -q --depth 1 tmp/docfx/serialization

mkdir -p tmp/docfx/build/src
cp -r ../src/NodaTime{,.Testing} tmp/docfx/build/src
cp -r ../*.snk tmp/docfx/build
cp -r tmp/docfx/serialization/src/NodaTime.Serialization.JsonNet tmp/docfx/build/src

cp docfx/global.json tmp/docfx
cp docfx/NodaTime.json tmp/docfx/build/src/NodaTime/project.json
cp docfx/NodaTime.Serialization.JsonNet.json tmp/docfx/build/src/NodaTime.Serialization.JsonNet/project.json
cp docfx/NodaTime.Testing.json tmp/docfx/build/src/NodaTime.Testing/project.json

cd tmp/docfx
dotnet restore build/src/NodaTime
dotnet restore build/src/NodaTime.Testing
dotnet restore build/src/NodaTime.Serialization.JsonNet
cd ../..

sed 's/..\/src/build\/src/g' < docfx/docfx.json > tmp/docfx/docfx.json
docfx tmp/docfx/docfx.json metadata -f 
cp docfx/toc.yml tmp/docfx/obj/unstable

# TODO: Add extra information (versions etc)

echo "Running main docfx build"
docfx build tmp/docfx/docfx.json
cp docfx/logo.svg tmp/docfx/_site
