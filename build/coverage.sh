#!/bin/bash

set -e

# Run coverage tests and optionally create a report.

declare -r ROOT=$(realpath $(dirname $0)/..)
declare -r TEST=$ROOT/src/NodaTime.Test
declare -r DOTCOVER_VERSION=2019.3.4
declare -r REPORTGENERATOR_VERSION=4.0.12

nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $DOTCOVER_VERSION JetBrains.dotCover.CommandLineTools

declare -r DOTCOVER=$ROOT/packages/JetBrains.dotCover.CommandLineTools.$DOTCOVER_VERSION/tools/dotCover.exe

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

# Run the tests under dotCover
(cd $TEST; 
 $DOTCOVER cover coverageparams.xml -ReturnTargetExitCode)

$DOTCOVER report --Source=$ROOT/coverage/NodaTime.dvcr --Output=$ROOT/coverage/coverage.xml --ReportType=DetailedXML ""

if [[ $1 == "--report" ]]
then
  nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $REPORTGENERATOR_VERSION ReportGenerator
  declare -r REPORTGENERATOR=$ROOT/packages/ReportGenerator.$REPORTGENERATOR_VERSION/tools/net47/ReportGenerator.exe
  
  $REPORTGENERATOR \
   -reports:$ROOT/coverage/coverage.xml \
   -targetdir:$ROOT/coverage/report \
   -verbosity:Error
fi
