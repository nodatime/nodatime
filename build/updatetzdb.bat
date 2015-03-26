@echo off
REM This is pretty hacky. It assumes you've got 7-zip (7z.exe) and wget on your
REM path... but it's better than nothing.

if "%2" == "" (
  echo Usage: updatetzdb tzdb-release-number path-to-cldr-mapping
  echo e.g. updatetzdb 2013h data\cldr\windowsZones-24.xml
  goto end
)

set DOWNLOADDIR=tmp\tzdb
set SRCDIR=..\src

IF EXIST %DOWNLOADDIR% rmdir /s /q %DOWNLOADDIR%
mkdir %DOWNLOADDIR%
wget http://www.iana.org/time-zones/repository/releases/tzdata%1.tar.gz -O %DOWNLOADDIR%\%1.tgz
7z x %DOWNLOADDIR%\%1.tgz -otzdbtmp
7z x %DOWNLOADDIR%\%1.tar -odata\tzdb\%1

REM Rebuild just in case...
msbuild "%SRCDIR%\NodaTime.TzdbCompiler\NodaTime.TzdbCompiler.csproj" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

%SRCDIR%\NodaTime.TzdbCompiler\bin\Release\NodaTime.TzdbCompiler.exe -o %SRCDIR%\NodaTime\TimeZones\Tzdb.nzd -s data\tzdb\%1 -w %2 -t %SRCDIR%\NodaTime.Test\TestData\tzdb-dump.txt

copy src\NodaTime\TimeZones\Tzdb.nzd www\tzdb\tzdb%1.nzd
echo http://nodatime.org/tzdb/tzdb%1.nzd > www\tzdb\latest.txt

del ..\www\tzdb\index.txt
FOR %%i IN (..\www\tzdb\*.nzd) DO echo http://nodatime.org/tzdb/%%~nxi >> ..\www\tzdb\index.txt

:end
