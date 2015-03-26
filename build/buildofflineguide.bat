@echo off

if "%1" == "" (
  echo Usage: buildofflineguide version-number
  goto end
)

set SRCDIR=..\www
set DESTDIR=tmp\docs
IF NOT EXIST tmp mkdir tmp
IF EXIST %DESTDIR% rmdir /s /q %DESTDIR%

xcopy /q /s /i %SRCDIR%\unstable\userguide %DESTDIR%
xcopy /q /s /i %SRCDIR%\_layouts %DESTDIR%\_layouts
xcopy /q /s /i %SRCDIR%\_plugins %DESTDIR%\_plugins
xcopy /q /s /i %SRCDIR%\css %DESTDIR%\css
xcopy /q /s /i %SRCDIR%\js %DESTDIR%\js
xcopy /q /s /i %SRCDIR%\fonts %DESTDIR%\fonts
copy %SRCDIR%\_config.yml %DESTDIR%
echo ug_version: %1 >> %DESTDIR%\_config.yml

REM Override the normal foundation layout
copy %DESTDIR%\_layouts\foundation-offline.html %DESTDIR%\_layouts\foundation.html


pushd %DESTDIR%
call jekyll build
popd

echo User guide is in %DESTDIR%\_site

:end
