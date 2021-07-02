#!/bin/bash

set -e

declare -r BUILD_DIR=$(realpath $(dirname $0))

DOTCOVER_PACKAGE_SUFFIX=.macos-x64 DOTCOVER_EXECUTABLE=./dotCover.sh $BUILD_DIR/coverage.sh "$@"
