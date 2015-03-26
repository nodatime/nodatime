@echo off

REM Runs the most recent benchmarks against all listed versions of
REM Noda Time, for easy comparison

set OUTDIR=tmp\benchmarks
set SRCDIR=..\src

if exist %OUTDIR% rmdir /s /q %OUTDIR%
mkdir %OUTDIR%

echo Running benchmarks for current source
msbuild /verbosity:quiet %SRCDIR%\NodaTime.Benchmarks\NodaTime.Benchmarks.csproj /p:Configuration=Release /t:Rebuild
IF ERRORLEVEL 1 EXIT /B 1
%SRCDIR%\NodaTime.Benchmarks\bin\Release\NodaTime.Benchmarks -x %OUTDIR%\current.xml -l Current
IF ERRORLEVEL 1 EXIT /B 1

CALL :runbenchmarks 1.0.1 nojson
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.1.1 nojson
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.2.0
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.3.1
IF ERRORLEVEL 1 EXIT /B 1

REM TODO: Make the version comparison a tool in a NuGet package, install and run it.
REM msbuild /verbosity:quiet %SRCDIR%\NodaTime.Benchmarks\NodaTime.Benchmarks.MultiVersionHtml.csproj /p:Configuration=Release /t:Rebuild
REM %SRCDIR%\NodaTime.Benchmarks.MultiVersionHtml\bin\Release\NodaTime.Benchmarks.MultiVersionHtml.exe %OUTDIR% %OUTDIR%\results.html

goto :end

:runbenchmarks
echo Running benchmarks for %1
nuget install NodaTime -Version %1 -OutputDirectory %SRCDIR%\packages
if NOT "%2"=="nojson" nuget install NodaTime.Serialization.JsonNet -Version %1 -OutputDirectory %SRCDIR%\packages
msbuild /verbosity:quiet %SRCDIR%\NodaTime.Benchmarks\NodaTime.Benchmarks.csproj /p:Configuration=Release /p:NodaVersion=%1 /t:Rebuild
IF ERRORLEVEL 1 EXIT /B 1
%SRCDIR%\NodaTime.Benchmarks\bin\Release\NodaTime.Benchmarks -x %OUTDIR%\v%1.xml -l v%1
IF ERRORLEVEL 1 EXIT /B 1

:end