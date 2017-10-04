#!/bin/bash

set -e

if [[ ! -d history ]]
then
  echo "Cloning history branch"
  git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b history history
fi

rm -rf tmp/docfx

echo "Copying metadata for previous versions"
for version in 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x 2.0.x; do
  mkdir -p tmp/docfx/obj/$version
  cp -r history/$version/api tmp/docfx/obj/$version
  cp docfx/toc.yml tmp/docfx/obj/$version
done

echo "Building metadata for current branch"
# We need to include the serialization docs (sometimes? unclear)

git clone https://github.com/nodatime/nodatime.serialization.git -q --depth 1 tmp/docfx/serialization

mkdir -p tmp/docfx/unstable/src
cp -r ../src/NodaTime{,.Testing} tmp/docfx/unstable/src
cp -r ../*.snk tmp/docfx/unstable
cp -r tmp/docfx/serialization/src/NodaTime.Serialization.JsonNet tmp/docfx/unstable/src

# Do the build for unstable so we can get annotations
cd tmp/docfx/unstable/src
dotnet restore NodaTime
dotnet restore NodaTime.Testing
dotnet restore NodaTime.Serialization.JsonNet
dotnet build NodaTime/NodaTime.csproj
dotnet build NodaTime.Testing/NodaTime.Testing.csproj
dotnet build NodaTime.Serialization.JsonNet/NodaTime.Serialization.JsonNet.csproj
cd ../../../..

cp -r docfx/template tmp/docfx
cp docfx/docfx-unstable.json tmp/docfx/docfx.json
docfx metadata tmp/docfx/docfx.json -f 
cp docfx/toc.yml tmp/docfx/obj/unstable

# Create diffs between versions and other annotations

dotnet restore Tools.sln
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.0.x tmp/docfx/obj/1.1.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.1.x tmp/docfx/obj/1.2.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.2.x tmp/docfx/obj/1.3.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.3.x tmp/docfx/obj/1.4.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.4.x tmp/docfx/obj/2.0.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/2.0.x tmp/docfx/obj/unstable

# Extract annotations
dotnet run -p DocfxAnnotationGenerator/DocfxAnnotationGenerator.csproj -- \
    tmp/docfx history/packages tmp/docfx/unstable/src 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x 2.0.x unstable

# Extract snippets from NodaTime.Demo (unstable only, for now)
# Make sure we've built everything, just to start with...
# (We could probably just build NodaTime.Demo... but we need the
# built file to reference.)
dotnet restore ../src/NodaTime-All.sln
dotnet build ../src/NodaTime-All.sln
dotnet run -p SnippetExtractor/SnippetExtractor.csproj -- ../src/NodaTime-All.sln NodaTime.Demo tmp/docfx/obj/unstable/overwrite

echo "Running main docfx build"
docfx build tmp/docfx/docfx.json
cp docfx/logo.svg tmp/docfx/_site
