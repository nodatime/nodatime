# This script is designed to be sourced from autobuildweb.sh.
# It allows the second phase of the script to be updated without
# refetching the file for the first phase.

# Build site and run smoke tests
echo $commit > $root/$commit/nodatime/src/NodaTime.Web/wwwroot/commit.txt

(cd $root/$commit/nodatime/build; ./buildweb.sh)

echo "Build and test successful. Pushing."

(cd $root/$commit/nodatime;
 cp build/nodatime.org/Dockerfile src/NodaTime.Web/bin/Release/netcoreapp2.2/publish;
 gcloud.cmd container builds submit \
   --config=build/nodatime.org/cloudbuild.yaml \
   --substitutions=COMMIT_SHA="$commit" \
   src/NodaTime.Web/bin/Release/netcoreapp2.2/publish)

