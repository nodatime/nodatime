#!/bin/bash

set -e

source docfx_functions.sh
install_docfx

if [[ ! -d history ]]
then
  echo "Cloning history branch"
  git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b history history
fi

rm -rf tmp/docfx

echo "Copying metadata for previous versions"
for version in 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x 2.0.x 2.1.x 2.2.x; do
  mkdir -p tmp/docfx/obj/$version
  cp -r history/$version/api tmp/docfx/obj/$version
  if [[ -d history/$version/overwrite ]]
  then
    cp -r history/$version/overwrite tmp/docfx/obj/$version
  fi
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
dotnet build NodaTime
dotnet build NodaTime.Testing
dotnet build NodaTime.Serialization.JsonNet
cd ../../../..

cp -r docfx/template tmp/docfx
cp docfx/docfx-unstable.json tmp/docfx/docfx.json
"$DOCFX" metadata tmp/docfx/docfx.json -f 
cp docfx/toc.yml tmp/docfx/obj/unstable

# Create diffs between versions and other annotations

dotnet restore Tools.sln
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/1.0.x tmp/docfx/obj/1.1.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/1.1.x tmp/docfx/obj/1.2.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/1.2.x tmp/docfx/obj/1.3.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/1.3.x tmp/docfx/obj/1.4.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/1.4.x tmp/docfx/obj/2.0.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/2.0.x tmp/docfx/obj/2.1.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/2.1.x tmp/docfx/obj/2.2.x
dotnet run -p ReleaseDiffGenerator -- tmp/docfx/obj/2.2.x tmp/docfx/obj/unstable

# Extract annotations
dotnet run -p DocfxAnnotationGenerator -- \
    tmp/docfx history/packages tmp/docfx/unstable/src 1.0.x 1.1.x 1.2.x 1.3.x 1.4.x 2.0.x 2.1.x 2.2.x unstable

# Extract snippets from NodaTime.Demo (unstable only, for now)
# Make sure we've built everything, just to start with...
# (We could probably just build NodaTime.Demo... but we need the
# built file to reference.)
dotnet build ../src/NodaTime-All.sln
dotnet run -p SnippetExtractor -- ../src/NodaTime-All.sln NodaTime.Demo tmp/docfx/obj/unstable/overwrite

echo "Running main docfx build"
"$DOCFX" build tmp/docfx/docfx.json
cp docfx/logo.svg tmp/docfx/_site
