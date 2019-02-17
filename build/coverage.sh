#!/bin/bash

set -e

# Run coverage tests and optionally create a report.

declare -r ROOT=$(realpath $(dirname $0)/..)
declare -r TEST=$ROOT/src/NodaTime.Test
declare -r DOTCOVER_VERSION=2018.3.3
declare -r REPORTGENERATOR_VERSION=4.0.12

nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $DOTCOVER_VERSION JetBrains.dotCover.CommandLineTools

declare -r DOTCOVER=$ROOT/packages/JetBrains.dotCover.CommandLineTools.$DOTCOVER_VERSION/tools/dotCover.exe

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

# dotCover fails with the current .NET Core SDK preview,
# so we make sure we've built with the preview, then use
# a global.json file to pin to the stable SDK
rm -f $TEST/global.json
dotnet build -c Release $TEST
cp $ROOT/build/global.json.old $TEST/global.json

# Run the tests under dotCover
(cd $TEST; 
 $DOTCOVER cover coverageparams.xml /ReturnTargetExitCode)

# Clean up
rm -f $TEST/global.json

$DOTCOVER report -Source=$ROOT/coverage/NodaTime.dvcr -Output=$ROOT/coverage/coverage.xml -ReportType=DetailedXML ""

if [[ $1 == "--report" ]]
then
  nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $REPORTGENERATOR_VERSION ReportGenerator
  declare -r REPORTGENERATOR=$ROOT/packages/ReportGenerator.$REPORTGENERATOR_VERSION/tools/net47/ReportGenerator.exe
  
  $REPORTGENERATOR \
   -reports:$ROOT/coverage/coverage.xml \
   -targetdir:$ROOT/coverage/report \
   -verbosity:Error
fi
