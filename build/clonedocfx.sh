#!/bin/sh
# Clones the docfx/ directory from the production (prebuilt) nodatime.org
# repository, and adds a symlink in src/NodaTime.Web/docfx, allowing the web
# site to be run locally.

set -e

cd $(dirname $0)/..
readonly ROOT=$(pwd)

readonly REPO=docfx-clone
readonly SUBDIR=docfx

# Fetch the docfx repo into build/tmp/docfx-clone/
cd ${ROOT}/build/
rm -rf tmp/${REPO}/
mkdir -p tmp/
cd tmp/
git init ${REPO}
cd ${REPO}
git remote add origin https://github.com/nodatime/nodatime.org.git
git config core.sparsecheckout true
echo "${SUBDIR}/*" >> .git/info/sparse-checkout
git pull --depth=1 origin master

# Now replace the docfx path under NodaTime.Web.
cd ${ROOT}/src/NodaTime.Web/
rm -rf ${SUBDIR}
ln -s ../../build/tmp/${REPO}/${SUBDIR}/ ${SUBDIR}
