#!/bin/bash

set -e

# This is currently ugly as the 1.x versions build in different ways to
# 2.0.x and master. Once docfx understands csproj files, we should be able
# to simplify this.
declare -r PREVIOUS_VERSIONS="1.0.x 1.1.x 1.2.x 1.3.x 2.0.x"
echo "Fetching previous versions from source control if necessary"

if [[ ! -d history ]]
then
  mkdir history
  # 1.x...
  for version in 1.0.x 1.1.x 1.2.x 1.3.x
  do
    echo "Cloning $version"
    git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b $version history/$version
  done

  # 2.0...
  echo "Cloning 2.0"
  git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b 2.0.x history/2.0.x
  echo "Cloning serialization"
  git clone https://github.com/nodatime/nodatime.serialization.git -q --depth 1 history/serialization
  git -C history/serialization checkout NodaTime.Serialization.JsonNet-2.0.0
  cp -r history/serialization/src/NodaTime.Serialization.JsonNet history/2.0.x/src
  echo "Preparing for docfx of 2.0"
  cp docfx/global.json history/2.0.x
  cp docfx/NodaTime.json history/2.0.x/src/NodaTime/project.json
  cp docfx/NodaTime.Serialization.JsonNet.json history/2.0.x/src/NodaTime.Serialization.JsonNet/project.json
  cp docfx/NodaTime.Testing.json history/2.0.x/src/NodaTime.Testing/project.json
  cd history/2.0.x
  dotnet restore src/NodaTime
  dotnet restore src/NodaTime.Testing
  dotnet restore src/NodaTime.Serialization.JsonNet
  cd ../..  
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

for version in 1.0.x 1.1.x 1.2.x 1.3.x; do
  echo "Building metadata for $version"
  rm -rf history/$version/api
  mkdir -p tmp/docfx/obj/$version
  cp docfx/docfx-csproj.json history/$version/docfx.json
  docfx metadata history/$version/docfx.json -f
  cp -r history/$version/api tmp/docfx/obj/$version
  cp docfx/toc.yml tmp/docfx/obj/$version
done

echo "Building metadata for 2.0.x"
rm -rf history/2.0.x/api
mkdir -p tmp/docfx/obj/2.0.x
sed 's/..\/src/src/g' < docfx/docfx.json | sed 's/obj\/unstable\///g' > history/2.0.x/docfx.json
docfx metadata history/2.0.x/docfx.json -f
cp -r history/2.0.x/api tmp/docfx/obj/2.0.x
cp docfx/toc.yml tmp/docfx/obj/2.0.x

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
docfx metadata tmp/docfx/docfx.json -f 
cp docfx/toc.yml tmp/docfx/obj/unstable

# Create diffs between versions
dotnet restore DocfxYamlLoader
dotnet restore ReleaseDiffGenerator

dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.0.x tmp/docfx/obj/1.1.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.1.x tmp/docfx/obj/1.2.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.2.x tmp/docfx/obj/1.3.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.3.x tmp/docfx/obj/2.0.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/2.0.x tmp/docfx/obj/unstable

# TODO: Add extra information (versions etc)

echo "Running main docfx build"
docfx build tmp/docfx/docfx.json
cp docfx/logo.svg tmp/docfx/_site
