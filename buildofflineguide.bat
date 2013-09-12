@echo off

if "%1" == "" (
  echo Usage: buildrelease version-number
  goto end
)

IF EXIST tmpdocs rmdir /s /q tmpdocs
xcopy /q /s /i www\unstable\userguide tmpdocs
xcopy /q /s /i www\_layouts tmpdocs\_layouts
xcopy /q /s /i www\_plugins tmpdocs\_plugins
xcopy /q /s /i www\css tmpdocs\css
xcopy /q /s /i www\fonts tmpdocs\fonts
copy www\_config.yml tmpdocs
echo ug_version: %1 >> tmpdocs\_config.yml

REM Override the normal foundation layout
copy tmpdocs\_layouts\foundation-offline.html tmpdocs\_layouts\foundation.html

cd tmpdocs
call jekyll build
cd ..

:end
