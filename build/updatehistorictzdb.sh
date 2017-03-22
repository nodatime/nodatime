#!/bin/bash

set -e

declare -r GSUTIL=gsutil.cmd
declare -r ROOT=$(realpath $(dirname $0)/..)

rm -rf tmp
mkdir -p tmp/releases
cd tmp/releases

# Work out the current release, fetch and extract it
declare -r RELEASE=$(\
    $GSUTIL ls gs://nodatime/releases | \
    grep -o -E 'NodaTime-1\.3\.[0-9]+\.zip' | \
    sort -r | \
    head -n 1 | \
    sed s/NodaTime-// | \
    sed s/.zip//)
echo "Fetching and expanding release ${RELEASE}"
$GSUTIL -q cp gs://nodatime/releases/NodaTime-${RELEASE}.zip .
$GSUTIL -q cp gs://nodatime/releases/NodaTime-${RELEASE}-src.zip .
curl -s -S https://api.nuget.org/packages/nodatime.${RELEASE}.nupkg -o nodatime.${RELEASE}.nupkg
unzip -q NodaTime-${RELEASE}.zip
unzip -q NodaTime-${RELEASE}-src.zip
unzip -q nodatime.${RELEASE}.nupkg -d NodaTime-${RELEASE}-nupkg

# Handy "increment version number" code from http://stackoverflow.com/questions/8653126
declare -r NEW_RELEASE=`echo $RELEASE | perl -pe 's/^((\d+\.)*)(\d+)(.*)$/$1.($3+1).$4/e'`

# Update all the files that need updating
echo "Updating to reflect ${NEW_RELEASE}"
mv NodaTime-${RELEASE} NodaTime-${NEW_RELEASE}
mv NodaTime-${RELEASE}-src NodaTime-${NEW_RELEASE}-src
cp $ROOT/src/NodaTime/TimeZones/Tzdb.nzd NodaTime-${NEW_RELEASE}-src/src/NodaTime/TimeZones
sed -i s/\"$RELEASE\"/\"$NEW_RELEASE\"/g NodaTime-${NEW_RELEASE}-src/src/NodaTime/Properties/AssemblyInfo.cs  
sed -i s/\<version\>$RELEASE\</\<version\>$NEW_RELEASE\</g NodaTime-${NEW_RELEASE}-src/src/NodaTime/NodaTime.nuspec
zip -q -r -9 NodaTime-${NEW_RELEASE}-src.zip NodaTime-${NEW_RELEASE}-src

# Perform the build
echo "Building ${NEW_RELEASE}"
cp "../../../NodaTime Release.snk" NodaTime-${NEW_RELEASE}-src
msbuild.exe NodaTime-${NEW_RELEASE}-src/src/NodaTime/NodaTime.csproj /property:Configuration="Signed Release" /verbosity:quiet
msbuild.exe NodaTime-${NEW_RELEASE}-src/src/NodaTime/NodaTime.csproj /property:Configuration="Signed Release Portable"  /verbosity:quiet
cp NodaTime-${RELEASE}-nupkg/lib/net35-Client/NodaTime.xml "NodaTime-${NEW_RELEASE}-src/src/NodaTime/bin/Signed Release"
cp NodaTime-${RELEASE}-nupkg/lib/portable*/NodaTime.xml "NodaTime-${NEW_RELEASE}-src/src/NodaTime/bin/Signed Release Portable"

# Build the NuGet package and new binary zip
echo "Packaging ${NEW_RELEASE}"
nuget pack NodaTime-${NEW_RELEASE}-src/src/NodaTime/NodaTime.nuspec -Symbols
cp "NodaTime-${NEW_RELEASE}-src/src/NodaTime/bin/Signed Release"/NodaTime.dll NodaTime-${NEW_RELEASE}
cp "NodaTime-${NEW_RELEASE}-src/src/NodaTime/bin/Signed Release Portable"/NodaTime.dll NodaTime-${NEW_RELEASE}/Portable
zip -q -r -9 NodaTime-${NEW_RELEASE}.zip NodaTime-${NEW_RELEASE}

cd ../..

echo "Done. Remaining tasks:"
echo "- Push package to nuget"
echo "- Copy src/binary releases to GCS"
