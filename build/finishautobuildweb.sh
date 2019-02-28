# This script is designed to be sourced from autobuildweb.sh.
# It allows the second phase of the script to be updated without
# refetching the file for the first phase.

# Build site and run smoke tests
(cd $root/$commit/nodatime/build; ./buildweb.sh ../../nodatime.org)

echo "Build and test successful. Pushing."

echo $commit > $root/$commit/nodatime.org/wwwroot/commit.txt

# Commit and push
# Ignore anything in .gitignore when adding files
(cd $root/$commit/nodatime.org;
 git add --all -f
 git commit -a -m "Regenerate from main repo commit $commit";
 git push)

echo "Building container on Google Cloud Container Builder."

(cd $root/$commit/nodatime;
 cp build/nodatime.org/Dockerfile src/NodaTime.Web/bin/Release/netcoreapp2.2/publish;
 gcloud.cmd container builds submit \
   --config=build/nodatime.org/cloudbuild.yaml \
   --substitutions=COMMIT_SHA="$commit" \
   src/NodaTime.Web/bin/Release/netcoreapp2.2/publish)

