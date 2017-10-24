#!/bin/bash

set -e

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-1.4.sh tzdb-release-number"
  echo "e.g. update-1.4.sh 2013h"
  exit 1
fi

declare -r TZDB_RELEASE=$1
declare -r GSUTIL=gsutil.cmd
# Note: due to the spaces, this has to be invoked as "$MSBUILD". I
# haven't worked out how to avoid that...
declare -r MSBUILD="/c/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe"
declare -r ROOT=$(realpath $(dirname $0)/../..)

rm -rf tmp-1.4
mkdir tmp-1.4
cd tmp-1.4

# Layout of tmp directory:
# - nodatime: git repo
# - old: previous zip and nupkg files
# - output: final zip and nupkg files
git clone https://github.com/nodatime/nodatime.git -b 1.4.x
mkdir old
mkdir output
declare -r OUTPUT="$(realpath $PWD/output)"

# Work out the current release, fetch and extract it
declare -r RELEASE=$(\
    $GSUTIL ls gs://nodatime/releases | \
    grep -o -E 'NodaTime-1\.4\.[0-9]+\.zip' | \
    sort -r | \
    head -n 1 | \
    sed s/NodaTime-// | \
    sed s/.zip//)
# Handy "increment version number" code from http://stackoverflow.com/questions/8653126
declare -r NEW_RELEASE=`echo $RELEASE | perl -pe 's/^((\d+\.)*)(\d+)(.*)$/$1.($3+1).$4/e'`

# We need the previous zip file for the relevant XML documentation files, which
# were post-processed with Sandcastle.
echo "Fetching and expanding release ${RELEASE}"
cd old
$GSUTIL -q cp gs://nodatime/releases/NodaTime-${RELEASE}.zip .
unzip -q NodaTime-${RELEASE}.zip
cp -r NodaTime-${RELEASE} "$OUTPUT/NodaTime-${NEW_RELEASE}"
cd ..

# Make sure the release key is present.
cp "${ROOT}/NodaTime Release.snk" nodatime

# Update the source code in the repo
cd nodatime
"$MSBUILD" src/NodaTime-All.sln
src/NodaTime.Tools.SetVersion/bin/Debug/SetVersion.exe ${NEW_RELEASE}
cp "${ROOT}/src/NodaTime/TimeZones/Tzdb.nzd" src/NodaTime/TimeZones

# Commit and tag the change
git commit -a -m "Update to TZDB ${TZDB_RELEASE} for release ${NEW_RELEASE}"
git tag ${NEW_RELEASE}

# Build the code
"$MSBUILD" src/NodaTime-All.sln /property:Configuration="Signed Release" /verbosity:quiet
"$MSBUILD" src/NodaTime-All.sln /property:Configuration="Signed Release Portable" /verbosity:quiet

# Copy the XML documentation from the previous build
cp ../old/NodaTime-${RELEASE}/NodaTime.xml "src/NodaTime/bin/Signed Release"
cp ../old/NodaTime-${RELEASE}/NodaTime.Testing.xml "src/NodaTime.Testing/bin/Signed Release"
cp ../old/NodaTime-${RELEASE}/NodaTime.Serialization.JsonNet.xml "src/NodaTime.Serialization.JsonNet/bin/Signed Release"

echo "Packaging..."
# NuGet packages
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime/NodaTime.nuspec -Symbols
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime.Testing/NodaTime.Testing.nuspec -Symbols
nuget pack -OutputDirectory "$OUTPUT" src/NodaTime.Serialization.JsonNet/NodaTime.Serialization.JsonNet.nuspec -Symbols

# Source zip file
git archive ${NEW_RELEASE} -o "$OUTPUT"/NodaTime-${NEW_RELEASE}-src.zip --prefix=NodaTime-${NEW_RELEASE}-src/

# Binary zip file
# We could just copy the NodaTime.dll file, as the code for the others hasn't changed, but
# let's keep it consistent with the NuGet package
declare -r OUTBIN="$OUTPUT/NodaTime-${NEW_RELEASE}"
cp "src/NodaTime/bin/Signed Release/NodaTime.dll" "${OUTBIN}"
cp "src/NodaTime.Testing/bin/Signed Release/NodaTime.Testing.dll" "${OUTBIN}"
cp "src/NodaTime.Serialization.JsonNet/bin/Signed Release/NodaTime.Serialization.JsonNet.dll" "${OUTBIN}"
cp "src/NodaTime/bin/Signed Release Portable/NodaTime.dll" "${OUTBIN}/Portable"
cp "src/NodaTime.Testing/bin/Signed Release Portable/NodaTime.Testing.dll" "${OUTBIN}/Portable"
cp "src/NodaTime.Serialization.JsonNet/bin/Signed Release Portable/NodaTime.Serialization.JsonNet.dll" "${OUTBIN}/Portable"
cd "$OUTPUT"
zip -q -r -9 NodaTime-${NEW_RELEASE}.zip NodaTime-${NEW_RELEASE}
rm -rf NodaTime-${NEW_RELEASE}

cd ../..

echo "Done. Remaining tasks:"
echo "- Push package to nuget"
echo "- Copy src/binary releases to GCS"
echo "- Push commit to github: git push origin 1.4.x"
echo "- Push tag to github: git push --tags origin"
