#!/bin/bash

set -eu -o pipefail

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-3.0.sh tzdb-release-number"
  echo "e.g. update-3.0.sh 2013h"
  exit 1
fi

declare -r TZDB_RELEASE=$1
declare -r GSUTIL=gsutil.cmd
declare -r ROOT=$(realpath $(dirname $0)/../..)

rm -rf tmp-3.0
mkdir tmp-3.0
cd tmp-3.0

# Layout of tmp directory:
# - nodatime: git repo
# - old: previous zip and nupkg files
# - output: final zip and nupkg files
git clone https://github.com/nodatime/nodatime.git -b 3.0.x --depth 1
mkdir old
mkdir output
declare -r OUTPUT="$(realpath $PWD/output)"

# Work out the current release, fetch and extract it.
# We use "ls -l" to include the release date in the listing,
# then sed to remove the file size part, then reverse sort.
declare -r RELEASE=$(\
    $GSUTIL ls -l gs://nodatime/releases | \
    sed -E 's/^ +[0-9]+ +//g' | \
    sort -r | \
    grep -o -E 'NodaTime-3\.0\.[0-9]+\.zip' | \
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
sed -i s/\>${RELEASE}\</\>${NEW_RELEASE}\</g Directory.Build.props
cp "${ROOT}/src/NodaTime/TimeZones/Tzdb.nzd" src/NodaTime/TimeZones

# Don't update the XML schema test file; this only needs to change when there's a new
# time zone, and the 3.0.x schema uses simple types instead of complex ones, so the
# tests fail.
# cp "${ROOT}/src/NodaTime.Test/Xml/XmlSchemaTest.XmlSchema.approved.xml" src/NodaTime.Test/Xml

# Commit and tag the change
git commit -a -m "Update to TZDB ${TZDB_RELEASE} for release ${NEW_RELEASE}"
git tag ${NEW_RELEASE}

# Make sure the packages end up with suitable embedded paths
export ContinuousIntegrationBuild=true

# Build and package the code
echo "Packaging..."
dotnet pack -o "$OUTPUT" -c Release src/NodaTime.sln

# Source zip file
git archive ${NEW_RELEASE} -o "$OUTPUT"/NodaTime-${NEW_RELEASE}-src.zip --prefix=NodaTime-${NEW_RELEASE}-src/

# Binary zip file
declare -r OUTBIN="$OUTPUT/NodaTime-${NEW_RELEASE}"
cp src/NodaTime/bin/Release/netstandard2.0/NodaTime.dll "${OUTBIN}"
cp src/NodaTime.Testing/bin/Release/netstandard2.0/NodaTime.Testing.dll "${OUTBIN}"

cd "$OUTPUT"
zip -q -r -9 NodaTime-${NEW_RELEASE}.zip NodaTime-${NEW_RELEASE}
rm -rf NodaTime-${NEW_RELEASE}

cd ../..

echo "Done. Remaining tasks:"
echo "- Push package to nuget"
echo "- Copy src/binary releases to GCS"
echo "- Push commit to github: git push origin 3.0.x"
echo "- Push tag to github: git push --tags origin"
