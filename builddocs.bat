@echo off
REM TODO: Don't do this in a batch file!

REM Really force a clean build...
rmdir /s /q src\NodaTime\bin\Release

msbuild "src\NodaTime.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild "tools\NodaTime.Tools.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

tools\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs tools\userguide-src docs\userguide
IF ERRORLEVEL 1 EXIT /B 1
tools\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs tools\developer-src docs\developer

IF ERRORLEVEL 1 EXIT /B 1

msbuild NodaTime.shfbproj
IF ERRORLEVEL 1 EXIT /B 1
