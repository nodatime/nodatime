#!/bin/bash

set -e

# Run coverage tests and optionally create a report.

declare -r ROOT=$(realpath $(dirname $0)/..)
declare -r DOTCOVER_VERSION=2017.1.20170613.162720 
declare -r REPORTGENERATOR_VERSION=2.5.10

nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $DOTCOVER_VERSION JetBrains.dotCover.CommandLineTools

declare -r DOTCOVER=$ROOT/packages/JetBrains.dotCover.CommandLineTools.$DOTCOVER_VERSION/tools/dotCover.exe

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

# Run just the net451 tests under dotCover
(cd $ROOT/src/NodaTime.Test; 
 $DOTCOVER cover coverageparams.xml /ReturnTargetExitCode)

$DOTCOVER report -Source=$ROOT/coverage/NodaTime.dvcr -Output=$ROOT/coverage/coverage.xml -ReportType=DetailedXML ""

if [[ $1 == "--report" ]]
then
  nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $REPORTGENERATOR_VERSION ReportGenerator
  declare -r REPORTGENERATOR=$ROOT/packages/ReportGenerator.$REPORTGENERATOR_VERSION/tools/ReportGenerator.exe
  
  $REPORTGENERATOR \
   -reports:$ROOT/coverage/coverage.xml \
   -targetdir:$ROOT/coverage/report \
   -verbosity:Error
fi
