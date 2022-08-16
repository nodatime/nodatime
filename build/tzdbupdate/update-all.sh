#!/bin/bash

set -eu -o pipefail

cd $(dirname $0)

if [[ "$1" = "" ]]
then
  echo "Usage: update-all.sh tzdb-release-number"
  echo "e.g. update-all.sh 2013h"
  exit 1
fi

./update-main.sh $1

rm -rf tmp-gcs
rm -rf tmp-nuget

mkdir tmp-gcs
mkdir tmp-nuget

for version in 3.1
do
  echo "Updating ${version}"
  ./update-${version}.sh $1
  (cd tmp-$version/nodatime; git push origin ${version}.x; git push --tags origin)
  cp tmp-$version/output/*.nupkg tmp-nuget
done

echo "Copying nzd file to storage"
gsutil.cmd cp ../../src/NodaTime/TimeZones/Tzdb.nzd gs://nodatime/tzdb/tzdb$1.nzd

# Symbol packages appear to be ineffective at the moment; best to just
# remove them (if any are even created; we don't use them now).
rm -f tmp-nuget/*.symbols.nupkg

echo "Remaining task - push nuget files:"
echo "cd tmp-nuget"
echo "for pkg in *.nupkg; do dotnet nuget push -s https://api.nuget.org/v3/index.json -k API_KEY_HERE \$pkg; done"
