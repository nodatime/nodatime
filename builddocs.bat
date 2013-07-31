@echo off
REM TODO: Don't do this in a batch file!

REM Really force a clean build...
rmdir /s /q src\NodaTime\bin\Release

msbuild "src\NodaTime-All.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

REM TODO: Jekyll build steps, and packaging.
REM Jekyll integration at the moment.
REM src\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs src\docs\userguide\project.xml docs\userguide
REM IF ERRORLEVEL 1 EXIT /B 1

msbuild NodaTime.shfbproj
IF ERRORLEVEL 1 EXIT /B 1
