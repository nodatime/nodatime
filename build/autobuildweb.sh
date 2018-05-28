#!/bin/bash

# This script is designed to be run on a scheduled basis, to build and deploy the web site
# in an automated manner. It takes the following steps, with input as a
# "web site build root directory" (henceforth known as $root).
#
# - Find the latest commit for the main repo
# - If $root/$commit exists, assume there have been no changes, and abort
# - Make a shallow clone (--depth 1) of the main repo to $root/$commit/nodatime
# - Make a shallow clone (--depth 1) of the nodatime.org repo to $root/$commit/nodatime.org
# - Run the fetched finishautobuildweb.sh to complete the remainder of the build
#
# It is anticipated that this file will not need to change often; it's effectively bootstrapping.

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

# Hand off to the second part of the build
source $root/$commit/nodatime/build/finishautobuildweb.sh
