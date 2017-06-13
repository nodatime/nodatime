#!/bin/bash

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)

if [[ "$3" = "" ]]
then
  echo "Usage: runbenchmarks.sh <repo-directory> <results-directory> [--upload] <target-framework> [target-framework...]"
  echo "e.g. runbenchmarks.sh benchmark-tmp benchmark-results --upload netcoreapp1.1 net45"
  echo "If --upload is specified, BenchmarkUploader will be run if benchmarks are successful"
  exit 1
fi

declare -r REPO=$1
declare -r RESULTS=$(realpath $2)

shift 2

rm -rf $REPO
git clone https://github.com/nodatime/nodatime.git $REPO --depth=1

declare -r COMMIT=$(git -C $REPO rev-parse HEAD)

if [[ -d $RESULTS/$COMMIT ]]
then
  echo "Benchmarks for commit $COMMIT already run. Exiting."
  exit 0
fi

cd $REPO/src
dotnet restore NodaTime-All.sln
cd NodaTime.Benchmarks

UPLOAD=""

if [[ $1 == "--upload" ]]
then
  shift
  UPLOAD="true"
fi

while (( "$#" ))
do
  TARGET_FRAMEWORK=$1
  OUTPUT=$RESULTS/$COMMIT/$TARGET_FRAMEWORK
  mkdir -p $OUTPUT
  echo "Running benchmarks for $TARGET_FRAMEWORK"
  rm -rf BenchmarkDotNet.Artifacts
  date -u -Iseconds > $OUTPUT/start.txt
  dotnet run -f $TARGET_FRAMEWORK -c Release -- --exporter=briefjson '*'
  date -u -Iseconds > $OUTPUT/end.txt
  for report in BenchmarkDotNet.Artifacts/results/*-report-brief.json
  do
    basereport=$(basename $report)
    cp $report $OUTPUT/${basereport/-report-brief/}
  done
  shift
done

if [[ $UPLOAD == "true" ]]
then
  echo "Uploading benchmarks"
  cd "$ROOT/build/BenchmarkUploader"
  dotnet restore
  dotnet run -- $RESULTS nodatime benchmarks/
fi
