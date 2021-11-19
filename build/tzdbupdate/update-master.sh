#!/bin/bash

set -eu -o pipefail

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-master.sh tzdb-release-number"
  echo "e.g. updatemaster.sh 2013h"
  exit 1
fi

cd $(dirname $0)

declare -r ROOT=../..
declare -r SRCDIR=$ROOT/src
declare -r OUTPUT=$SRCDIR/NodaTime/TimeZones/Tzdb.nzd
declare -r DATADIR=$ROOT/data
declare -r WWWDIR=$SRCDIR/NodaTime.Web/wwwroot

echo "Building solution"
dotnet build -nologo -clp:NoSummary -v quiet $SRCDIR/NodaTime.sln

echo ""
echo "Generating NZD file"
dotnet run --project $SRCDIR/NodaTime.TzdbCompiler -f netcoreapp3.1 -- \
  -o $OUTPUT \
  -s https://data.iana.org/time-zones/releases/tzdata$1.tar.gz \
  -w $DATADIR/cldr \
  -x $SRCDIR/NodaTime.Test/Xml/XmlSchemaTest.XmlSchema.approved.xml \
  | grep -v "Skipping"

echo ""
echo "Testing"
dotnet build -nologo -clp:NoSummary -v quiet -c Release $SRCDIR/NodaTime.Test
dotnet test -nologo --no-build -c Release ../../src/NodaTime.Test

echo "Checking hashes"
rm -rf tmp-hashes
mkdir tmp-hashes

wget -q -O tmp-hashes/github-pages.txt https://nodatime.github.io/tzvalidate/tzdata$1-sha256.txt 2> /dev/null
dotnet run --project $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash | grep -v "Skipping" > tmp-hashes/local.txt
dotnet run --project $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash --noabbr | grep -v "Skipping" > tmp-hashes/local-noabbr.txt
dotnet run --project $SRCDIR/NodaTime.TzValidate.NzdCompatibility -- -s $OUTPUT --hash --noabbr | grep -v "Skipping" > tmp-hashes/local-noabbr-11.txt

echo ""
echo "Hash on github pages: $(< tmp-hashes/github-pages.txt)"
echo "Hash from new file: $(< tmp-hashes/local.txt)"
echo "Hash from new file without abbreviations: $(< tmp-hashes/local-noabbr.txt)"
echo "Hash from new file without abbreviations, using Noda Time 1.1: $(< tmp-hashes/local-noabbr-11.txt)"

# diff's exit code will stop the script at this point if the hashes don't match
diff tmp-hashes/github-pages.txt tmp-hashes/local.txt
diff tmp-hashes/local-noabbr.txt tmp-hashes/local-noabbr-11.txt

echo ""
echo "Success!"
echo "When you're ready, update Google Cloud Storage:"
echo "gsutil cp $OUTPUT gs://nodatime/tzdb/tzdb$1.nzd"
