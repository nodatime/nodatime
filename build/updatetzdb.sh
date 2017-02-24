#!/bin/bash

set -e

if [[ "$1" = "" ]]
then
  echo "Usage: updatetzdb.sh tzdb-release-number"
  echo "e.g. updatetzdb 2013h"
  exit 1
fi

SRCDIR=../src
OUTPUT=$SRCDIR/NodaTime/TimeZones/Tzdb.nzd
DATADIR=../data
WWWDIR=../src/NodaTime.Web/wwwroot

dotnet restore -v Error $SRCDIR
dotnet build $SRCDIR/{NodaTime,NodaTime.TzdbCompiler,NodaTime.TzValidate.NodaDump,NodaTime.TzValidate.NzdCompatibility} \
  > /dev/null

dotnet run -p $SRCDIR/NodaTime.TzdbCompiler -- \
  -o $OUTPUT \
  -s http://www.iana.org/time-zones/repository/releases/tzdata$1.tar.gz \
  -w $DATADIR/cldr \
  | grep -v "Skipping"
  
dotnet build $SRCDIR/{NodaTime,NodaTime.TzdbCompiler,NodaTime.TzValidate.NodaDump,NodaTime.TzValidate.NzdCompatibility} \
  > /dev/null
  
echo ""

echo Hash on github pages:
wget -q -O - http://nodatime.github.io/tzvalidate/tzdata$1-sha256.txt 2> /dev/null
echo Hash from new file:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash | grep -v "Skipping"
echo Hash from new file without abbreviations:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"
echo Hash from new file without abbreviations, using Noda Time 1.1:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NzdCompatibility -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"

echo ""
echo "When you're ready, update Google Cloud Storage:"
echo "gsutil cp $OUTPUT gs://nodatime/tzdb/tzdb$1.nzd"
