@echo off

REM Runs the most recent benchmarks against all listed versions of
REM Noda Time, for easy comparison

if exist tmpbenchmarks rmdir /s /q tmpbenchmarks
mkdir tmpbenchmarks

echo Running benchmarks for current source
msbuild /verbosity:quiet src\NodaTime.Benchmarks\NodaTime.Benchmarks.csproj /p:Configuration=Release /t:Rebuild
IF ERRORLEVEL 1 EXIT /B 1
src\NodaTime.Benchmarks\bin\Release\NodaTime.Benchmarks -x tmpbenchmarks\current.xml -l Current
IF ERRORLEVEL 1 EXIT /B 1

CALL :runbenchmarks 1.0.1 nojson
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.1.1 nojson
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.2.0
IF ERRORLEVEL 1 EXIT /B 1
CALL :runbenchmarks 1.3.1
IF ERRORLEVEL 1 EXIT /B 1
goto :end

:runbenchmarks
echo Running benchmarks for %1
nuget install NodaTime -Version %1 -OutputDirectory src\packages
if NOT "%2"=="nojson" nuget install NodaTime.Serialization.JsonNet -Version %1 -OutputDirectory src\packages
msbuild /verbosity:quiet src\NodaTime.Benchmarks\NodaTime.Benchmarks.csproj /p:Configuration=Release /p:NodaVersion=%1 /t:Rebuild
IF ERRORLEVEL 1 EXIT /B 1
src\NodaTime.Benchmarks\bin\Release\NodaTime.Benchmarks -x tmpbenchmarks\v%1.xml -l v%1
IF ERRORLEVEL 1 EXIT /B 1

:end