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

declare -r DOTCOVER=$ROOT/packages/jetbrains.dotcover.commandlinetools/$DOTCOVER_VERSION/tools/dotCover.exe

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

# Run the tests under dotCover
(cd $TEST; 
 $DOTCOVER cover coverageparams.xml -ReturnTargetExitCode)

$DOTCOVER report --Source=$ROOT/coverage/NodaTime.dvcr --Output=$ROOT/coverage/coverage.xml --ReportType=DetailedXML ""

if [[ $1 == "--report" ]]
then
  declare -r REPORTGENERATOR=$ROOT/packages/reportgenerator/$REPORTGENERATOR_VERSION/tools/net47/ReportGenerator.exe
  
  $REPORTGENERATOR \
   -reports:$ROOT/coverage/coverage.xml \
   -targetdir:$ROOT/coverage/report \
   -verbosity:Error
fi
