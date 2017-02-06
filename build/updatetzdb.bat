@echo off

if "%1" == "" (
  echo Usage: updatetzdb tzdb-release-number
  echo e.g. updatetzdb 2013h
  goto end
)

set SRCDIR=..\src
set DATADIR=..\data
set WWWDIR=..\src\NodaTime.Web\wwwroot

call dotnet restore -v Error %SRCDIR%
IF ERRORLEVEL 1 EXIT /B 1

call dotnet build %SRCDIR%\NodaTime.TzdbCompiler %SRCDIR%\NodaTime.TzValidate.NodaDump %SRCDIR%\NodaTime.TzValidate.NzdCompatibility
IF ERRORLEVEL 1 EXIT /B 1

dotnet run -p %SRCDIR%\NodaTime.TzdbCompiler -- -o %SRCDIR%\NodaTime\TimeZones\Tzdb.nzd -s http://www.iana.org/time-zones/repository/releases/tzdata%1.tar.gz -w %DATADIR%\cldr

copy %SRCDIR%\NodaTime\TimeZones\Tzdb.nzd %WWWDIR%\tzdb\tzdb%1.nzd
echo http://nodatime.org/tzdb/tzdb%1.nzd> %WWWDIR%\tzdb\latest.txt

del %WWWDIR%\tzdb\index.txt
FOR %%i IN (%WWWDIR%\tzdb\*.nzd) DO echo http://nodatime.org/tzdb/%%~nxi>> %WWWDIR%\tzdb\index.txt

echo Hash on github pages:
wget -q -O - http://nodatime.github.io/tzvalidate/tzdata%1%-sha256.txt 2> NUL
echo Hash from new file:
dotnet run -p %SRCDIR%\NodaTime.TzValidate.NodaDump -- -s %WWWDIR%\tzdb\tzdb%1.nzd --hash
echo Hash from new file without abbreviations:
dotnet run -p %SRCDIR%\NodaTime.TzValidate.NodaDump -- -s %WWWDIR%\tzdb\tzdb%1.nzd --hash --noabbr
echo Hash from new file without abbreviations, using Noda Time 1.1:
dotnet run -p %SRCDIR%\NodaTime.TzValidate.NzdCompatibility -- -s %WWWDIR%\tzdb\tzdb%1.nzd --hash --noabbr

:end
