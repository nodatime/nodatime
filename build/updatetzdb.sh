#!/bin/bash

set -e

if [[ "$1" = "" ]]
then
  echo "Usage: updatetzdb.sh tzdb-release-number"
  echo "e.g. updatetzdb 2013h"
  exit 1
fi

declare -r SRCDIR=../src
declare -r OUTPUT=$SRCDIR/NodaTime/TimeZones/Tzdb.nzd
declare -r DATADIR=../data
declare -r WWWDIR=../src/NodaTime.Web/wwwroot
declare -r PROJECTS="NodaTime NodaTime.TzdbCompiler NodaTime.TzValidate.NodaDump NodaTime.TzValidate.NzdCompatibility"

for proj in $PROJECTS
do
  dotnet restore -v quiet $SRCDIR/$proj
  dotnet build $SRCDIR/$proj
done

dotnet run -p $SRCDIR/NodaTime.TzdbCompiler/*.csproj -- \
  -o $OUTPUT \
  -s http://www.iana.org/time-zones/repository/releases/tzdata$1.tar.gz \
  -w $DATADIR/cldr \
  | grep -v "Skipping"
  
echo ""

echo Hash on github pages:
wget -q -O - http://nodatime.github.io/tzvalidate/tzdata$1-sha256.txt 2> /dev/null
echo Hash from new file:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump/*.csproj -- -s $OUTPUT --hash | grep -v "Skipping"
echo Hash from new file without abbreviations:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NodaDump/*.csproj -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"
echo Hash from new file without abbreviations, using Noda Time 1.1:
dotnet run -p $SRCDIR/NodaTime.TzValidate.NzdCompatibility/*.csproj -- -s $OUTPUT --hash --noabbr | grep -v "Skipping"

echo ""
echo "When you're ready, update Google Cloud Storage:"
echo "gsutil cp $OUTPUT gs://nodatime/tzdb/tzdb$1.nzd"
