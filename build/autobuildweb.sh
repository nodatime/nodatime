#!/bin/bash

# This script is designed to be run on a scheduled basis, to build and deploy the web site
# in an automated manner. It takes the following steps, with input as a
# "web site build root directory" (henceforth known as $root).
#
# - Find the latest commit for the main repo
# - If $root/$commit exists, assume there have been no changes, and abort
# - Make a shallow clone (--depth 1) of the main repo to $root/$commit/nodatime
# - Make a shallow clone (--depth 1) of the nodatime.org repo to $root/$commit/nodatime.org
# - Run $root/$commit/build/buildweb.sh $root/$commit/nodatime.org
# - In $root/$commit/nodatime.org
#   - Create a new commit
#   - Push to nodatime.org repo

set -e

if [[ "$1" = "" ]]
then
  echo "Usage: autobuildweb.sh output-directory"
  echo "e.g. autobuildweb.sh c:\\users\\jon\\NodaTimeWeb"
  exit 1
fi

declare -r root=$1
declare -r commit=$(curl -H Accept:application/vnd.github.VERSION.sha https://api.github.com/repos/nodatime/nodatime/commits/master)

if [[ -d $root/$commit ]]
then
  echo "Directory $root/$commit already exists. Aborting."
  exit 0
fi

# Clone repos
git clone https://github.com/nodatime/nodatime.git --depth 1 $root/$commit/nodatime
git clone https://github.com/nodatime/nodatime.org.git --depth 1 $root/$commit/nodatime.org

# Build site and run smoke tests
(cd $root/$commit/nodatime/build; ./buildweb.sh ../../nodatime.org &> buildweb.log)

echo "Build and test successful. Pushing."

# Commit and push
# Ignore anything in .gitignore when adding files
git -C $root/$commit/nodatime.org add --all -f
git -C $root/$commit/nodatime.org commit -a -m "Regenerate from main repo commit $commit"
git -C $root/$commit/nodatime.org push
