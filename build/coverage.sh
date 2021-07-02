#!/bin/bash

set -e

# Run coverage tests and optionally create a report.

declare -r ROOT=$(realpath $(dirname $0)/..)
declare -r TEST=$ROOT/src/NodaTime.Test
declare -r DOTCOVER_VERSION=2021.1.3
declare -r REPORTGENERATOR_VERSION=4.8.11

export REPORTGENERATOR_VERSION
export DOTCOVER_VERSION
dotnet restore $ROOT/build/Coverage.proj --packages $ROOT/packages

declare -r DOTCOVER_DIR=$ROOT/packages/jetbrains.dotcover.commandlinetools/$DOTCOVER_VERSION/tools

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

# Run the tests under dotCover
(cd $DOTCOVER_DIR; 
 ./dotCover.exe dotnet $TEST/coverageparams.xml --Output=$ROOT/coverage/coverage.xml --ReportType=DetailedXML --ReturnTargetExitCode -- test $TEST)

if [[ $1 == "--report" ]]
then
  declare -r REPORTGENERATOR=$ROOT/packages/reportgenerator/$REPORTGENERATOR_VERSION/tools/net5.0/ReportGenerator.dll
  
  dotnet $REPORTGENERATOR \
   -reports:$ROOT/coverage/coverage.xml \
   -targetdir:$ROOT/coverage/report \
   -verbosity:Error
fi
