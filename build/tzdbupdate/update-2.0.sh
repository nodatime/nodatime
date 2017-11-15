#!/bin/bash

set -e

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-2.0.sh tzdb-release-number"
  echo "e.g. update-2.0.sh 2013h"
  exit 1
fi

declare -r TZDB_RELEASE=$1
declare -r GSUTIL=gsutil.cmd
declare -r ROOT=$(realpath $(dirname $0)/../..)

rm -rf tmp-2.0
mkdir tmp-2.0
cd tmp-2.0

# Layout of tmp directory:
# - nodatime: git repo
# - old: previous zip and nupkg files
# - output: final zip and nupkg files
git clone https://github.com/nodatime/nodatime.git -b 2.0.x
mkdir old
mkdir output
declare -r OUTPUT="$(realpath $PWD/output)"

# Work out the current release, fetch and extract it
declare -r RELEASE=$(\
    $GSUTIL ls gs://nodatime/releases | \
    grep -o -E 'NodaTime-2\.0\.[0-9]+\.zip' | \
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

# Update the source code in the repo
cd nodatime
sed -i s/\>${RELEASE}\</\>${NEW_RELEASE}\</g src/NodaTime/NodaTime.csproj
sed -i s/\>${RELEASE}\</\>${NEW_RELEASE}\</g src/NodaTime.Testing/NodaTime.Testing.csproj
cp "${ROOT}/src/NodaTime/TimeZones/Tzdb.nzd" src/NodaTime/TimeZones

# Commit and tag the change
git commit -a -m "Update to TZDB ${TZDB_RELEASE} for release ${NEW_RELEASE}"
git tag ${NEW_RELEASE}

# Build the code
dotnet restore src/NodaTime-All.sln
dotnet build -c Release src/NodaTime-All.sln

echo "Packaging..."
# NuGet packages
dotnet pack -o "$OUTPUT" -c Release --no-build src/NodaTime-All.sln

# Source zip file
git archive ${NEW_RELEASE} -o "$OUTPUT"/NodaTime-${NEW_RELEASE}-src.zip --prefix=NodaTime-${NEW_RELEASE}-src/

# Binary zip file
declare -r OUTBIN="$OUTPUT/NodaTime-${NEW_RELEASE}"
cp src/NodaTime/bin/Release/net45/NodaTime.dll "${OUTBIN}"
cp src/NodaTime.Testing/bin/Release/net45/NodaTime.Testing.dll "${OUTBIN}"
cp src/NodaTime/bin/Release/netstandard1.3/NodaTime.dll "${OUTBIN}/Portable"
cp src/NodaTime.Testing/bin/Release/netstandard1.3/NodaTime.Testing.dll "${OUTBIN}/Portable"

cd "$OUTPUT"
zip -q -r -9 NodaTime-${NEW_RELEASE}.zip NodaTime-${NEW_RELEASE}
rm -rf NodaTime-${NEW_RELEASE}

cd ../..

echo "Done. Remaining tasks:"
echo "- Push package to nuget"
echo "- Copy src/binary releases to GCS"
echo "- Push commit to github: git push origin 2.0.x"
echo "- Push tag to github: git push --tags origin"
