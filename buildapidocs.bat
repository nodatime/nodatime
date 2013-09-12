@echo off
REM TODO: Don't do this in a batch file!

REM Really force a clean build...
rmdir /s /q src\NodaTime\bin\Release

msbuild "src\NodaTime-All.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild NodaTime.shfbproj
IF ERRORLEVEL 1 EXIT /B 1
