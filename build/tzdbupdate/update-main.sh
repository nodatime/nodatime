#!/bin/bash

set -eu -o pipefail

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-main.sh tzdb-release-number"
  echo "e.g. update-main.sh 2013h"
  exit 1
fi

cd $(dirname $0)

declare -r ROOT=../..
declare -r SRCDIR=$ROOT/src
declare -r OUTPUT=$SRCDIR/NodaTime/TimeZones/Tzdb.nzd
declare -r DATADIR=$ROOT/data
declare -r WWWDIR=$SRCDIR/NodaTime.Web/wwwroot

echo "Building solution"
dotnet build -nologo -clp:NoSummary -v quiet $SRCDIR/NodaTime.slnx

echo ""
echo "Generating NZD file"
dotnet run --project $SRCDIR/NodaTime.TzdbCompiler -f net10.0 -- \
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

declare -r HASH_URL=https://nodatime.github.io/tzvalidate/tzdata$1-sha256.txt
if ! wget -q -O tmp-hashes/github-pages.txt $HASH_URL
then
  echo "Failed to fetch expected hash from $HASH_URL (has tzvalidate been updated?)"
  exit 1
fi
dotnet run --project $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash | grep -v "Skipping" > tmp-hashes/local.txt
dotnet run --project $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash --noabbr | grep -v "Skipping" > tmp-hashes/local-noabbr.txt
dotnet run --project $SRCDIR/NodaTime.TzValidate.NzdCompatibility -- -s $OUTPUT --hash --noabbr | grep -v "Skipping" > tmp-hashes/local-noabbr-11.txt

echo ""
echo "Hash on github pages: $(< tmp-hashes/github-pages.txt)"
echo "Hash from new file: $(< tmp-hashes/local.txt)"
echo "Hash from new file without abbreviations: $(< tmp-hashes/local-noabbr.txt)"
echo "Hash from new file without abbreviations, using Noda Time 1.1: $(< tmp-hashes/local-noabbr-11.txt)"

echo ""

# Fails the script unless both files contain the same single SHA-256 hash.
# (Validating the format first prevents two empty or error-containing files
# from "matching".)
check_hashes() {
  local expected=$(tr -d '\r\n' < $1)
  local actual=$(tr -d '\r\n' < $2)
  for hash in "$expected" "$actual"
  do
    if [[ ! "$hash" =~ ^[0-9a-f]{64}$ ]]
    then
      echo "Invalid hash '$hash' when comparing $1 and $2"
      exit 1
    fi
  done
  if [[ "$expected" != "$actual" ]]
  then
    echo "Hash mismatch: $1 has $expected; $2 has $actual"
    exit 1
  fi
  echo "Hashes match: $1 and $2"
}

check_hashes tmp-hashes/github-pages.txt tmp-hashes/local.txt
check_hashes tmp-hashes/local-noabbr.txt tmp-hashes/local-noabbr-11.txt

echo ""
echo "Success!"
echo "When you're ready, update Google Cloud Storage:"
echo "gcloud storage cp $OUTPUT gs://nodatime/tzdb/tzdb$1.nzd"
