#!/bin/bash

set -e

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

dotnet restore -v quiet $SRCDIR/NodaTime.sln
dotnet build $SRCDIR/NodaTime.sln

dotnet run -p $SRCDIR/NodaTime.TzdbCompiler -- \
  -o $OUTPUT \
  -s https://data.iana.org/time-zones/releases/tzdata$1.tar.gz \
  -w $DATADIR/cldr \
  -x $SRCDIR/NodaTime.Test/Xml/XmlSchemaTest.XmlSchema.approved.xml \
  | grep -v "Skipping"

echo ""

dotnet test ../../src/NodaTime.Test --filter=TestCategory!=Slow

echo Hash on github pages:
wget -q -O - https://nodatime.github.io/tzvalidate/tzdata$1-sha256.txt 2> /dev/null
echo Hash from new file:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash | grep -v "Skipping"
echo Hash from new file without abbreviations:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"
echo Hash from new file without abbreviations, using Noda Time 1.1:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NzdCompatibility -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"

echo ""
echo "When you're ready, update Google Cloud Storage:"
echo "gsutil cp $OUTPUT gs://nodatime/tzdb/tzdb$1.nzd"
