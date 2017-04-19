#!/bin/bash

set -e

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

  # NodaTime
  wget --quiet -Ohistory/NodaTime-1.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.0.0
  wget --quiet -Ohistory/NodaTime-1.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.1.0
  wget --quiet -Ohistory/NodaTime-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.2.0
  wget --quiet -Ohistory/NodaTime-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/1.3.2
  wget --quiet -Ohistory/NodaTime-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime/2.0.0

  # NodaTime.Testing
  wget --quiet -Ohistory/NodaTime.Testing-1.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.0.0
  wget --quiet -Ohistory/NodaTime.Testing-1.1.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.1.0
  wget --quiet -Ohistory/NodaTime.Testing-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.2.0
  wget --quiet -Ohistory/NodaTime.Testing-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/1.3.2
  wget --quiet -Ohistory/NodaTime.Testing-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Testing/2.0.0

  # NodaTime.Serialization.JsonNet
  wget --quiet -Ohistory/NodaTime.Serialization.JsonNet-1.2.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/1.2.0
  wget --quiet -Ohistory/NodaTime.Serialization.JsonNet-1.3.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/1.3.2
  wget --quiet -Ohistory/NodaTime.Serialization.JsonNet-2.0.x.nupkg https://www.nuget.org/api/v2/package/NodaTime.Serialization.JsonNet/2.0.0

  # 2.0...
  echo "Cloning 2.0"
  git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b 2.0.x history/2.0.x
  echo "Cloning serialization"
  git clone https://github.com/nodatime/nodatime.serialization.git -q --depth 1 history/serialization
  git -C history/serialization checkout NodaTime.Serialization.JsonNet-2.0.0
  cp -r history/serialization/src/NodaTime.Serialization.JsonNet history/2.0.x/src
  echo "Preparing for docfx of 2.0"
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

for version in 1.0.x 1.1.x 1.2.x 1.3.x 2.0.x; do
  echo "Building metadata for $version"
  rm -rf history/$version/api
  mkdir -p tmp/docfx/obj/$version
  cp docfx/docfx-$version.json history/$version/docfx.json
  docfx metadata history/$version/docfx.json -f
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
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/1.3.x tmp/docfx/obj/2.0.x
dotnet run -p ReleaseDiffGenerator/ReleaseDiffGenerator.csproj -- tmp/docfx/obj/2.0.x tmp/docfx/obj/unstable
dotnet run -p DocfxAnnotationGenerator/DocfxAnnotationGenerator.csproj -- tmp/docfx history tmp/docfx/unstable/src 1.0.x 1.1.x 1.2.x 1.3.x 2.0.x unstable

echo "Running main docfx build"
docfx build tmp/docfx/docfx.json
cp docfx/logo.svg tmp/docfx/_site
