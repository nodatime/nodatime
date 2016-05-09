@echo off

if "%1" == "" (
  echo Usage: updatetzdb tzdb-release-number
  echo e.g. updatetzdb 2013h
  goto end
)

set SRCDIR=..\src
set DATADIR=..\data
set WWWDIR=..\www

call dnu restore %SRCDIR%
IF ERRORLEVEL 1 EXIT /B 1

call dnu build %SRCDIR%\NodaTime.TzdbCompiler
IF ERRORLEVEL 1 EXIT /B 1

dnx -p %SRCDIR%\NodaTime.TzdbCompiler run -o %SRCDIR%\NodaTime\TimeZones\Tzdb.nzd -s http://www.iana.org/time-zones/repository/releases/tzdata%1.tar.gz -w %DATADIR%\cldr -t %SRCDIR%\NodaTime.Test\TestData\tzdb-dump.txt

copy %SRCDIR%\NodaTime\TimeZones\Tzdb.nzd %WWWDIR%\tzdb\tzdb%1.nzd
echo http://nodatime.org/tzdb/tzdb%1.nzd> %WWWDIR%\tzdb\latest.txt

del %WWWDIR%\tzdb\index.txt
FOR %%i IN (%WWWDIR%\tzdb\*.nzd) DO echo http://nodatime.org/tzdb/%%~nxi>> %WWWDIR%\tzdb\index.txt

echo Hash on github pages:
wget -q -O - http://nodatime.github.io/tzvalidate/tzdata%1%-sha256.txt 2> NUL
echo Hash from new file:
dnx -p %SRCDIR%\NodaTime.TzValidate.NodaDump run -s %WWWDIR%\tzdb\tzdb%1.nzd --hash

:end
