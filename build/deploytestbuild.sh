#!/bin/bash

ROOT=$(git rev-parse --show-toplevel)
(cd $ROOT/src/NodaTime.Web;
 dotnet publish -c Debug;
 echo "Built at $(date -u -Iseconds)" > bin/Debug/netcoreapp2.2/publish/wwwroot/build.txt;
 cp $ROOT/build/nodatime.org/Dockerfile bin/Debug/netcoreapp2.2/publish;
 gcloud.cmd builds submit \
   --config=$ROOT/build/nodatime.org/testcloudbuild.yaml \
   $ROOT/src/NodaTime.Web/bin/Debug/netcoreapp2.2/publish)
