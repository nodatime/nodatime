#!/bin/bash

set -e

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: buildrelease.sh package-version"
  echo "e.g. buildrelease.sh 1.4.2"
  exit 1
fi

declare -r VERSION=$1
declare -r OUTPUT="$(realpath $PWD/releasebuild)"
declare -r MSBUILD="/c/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe"

rm -rf "$OUTPUT"
mkdir "$OUTPUT"
cd "$OUTPUT"

git clone https://github.com/nodatime/nodatime.git
cd nodatime
git checkout $VERSION

# Build the code
"$MSBUILD" src/NodaTime-All.sln /property:Configuration="Signed Release" /verbosity:quiet
"$MSBUILD" src/NodaTime-All.sln /property:Configuration="Signed Release Portable" /verbosity:quiet

echo "Packaging..."
# NuGet packages
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime/NodaTime.nuspec -Symbols
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime.Testing/NodaTime.Testing.nuspec -Symbols
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime.Serialization.JsonNet/NodaTime.Serialization.JsonNet.nuspec -Symbols

# Source zip file
git archive ${VERSION} -o "$OUTPUT"/NodaTime-$VERSION-src.zip --prefix=NodaTime-$VERSION-src/

# Binary zip file
# We could just copy the NodaTime.dll file, as the code for the others hasn't changed, but
# let's keep it consistent with the NuGet package
declare -r OUTBIN="$OUTPUT/NodaTime-${VERSION}"
mkdir "$OUTBIN"
mkdir "$OUTBIN/Portable"
cp readme.txt "$OUTBIN"
cp "src/NodaTime/bin/Signed Release/NodaTime.dll" "${OUTBIN}"
cp "src/NodaTime/bin/Signed Release/NodaTime.xml" "${OUTBIN}"
cp "src/NodaTime.Testing/bin/Signed Release/NodaTime.Testing.dll" "${OUTBIN}"
cp "src/NodaTime.Testing/bin/Signed Release/NodaTime.Testing.xml" "${OUTBIN}"
cp "src/NodaTime.Serialization.JsonNet/bin/Signed Release/NodaTime.Serialization.JsonNet.dll" "${OUTBIN}"
cp "src/NodaTime.Serialization.JsonNet/bin/Signed Release/NodaTime.Serialization.JsonNet.xml" "${OUTBIN}"
cp "src/NodaTime/bin/Signed Release Portable/NodaTime.dll" "${OUTBIN}/Portable"
cp "src/NodaTime.Testing/bin/Signed Release Portable/NodaTime.Testing.dll" "${OUTBIN}/Portable"
cp "src/NodaTime.Serialization.JsonNet/bin/Signed Release Portable/NodaTime.Serialization.JsonNet.dll" "${OUTBIN}/Portable"
cd "$OUTPUT"
zip -q -r -9 NodaTime-${VERSION}.zip NodaTime-${VERSION}
rm -rf NodaTime-${VERSION}
