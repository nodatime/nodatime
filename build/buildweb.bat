@echo off

if "%1" == "" (
  echo Usage: buildweb output-directory
  echo e.g. buildweb c:\users\jon\nodatime.org
  echo It is expected that the output directory already exists and is
  echo set up for git...
  goto end
)

set WEB_DIR=%1

call :clean
IF ERRORLEVEL 1 EXIT /B 1
call buildapidocs
IF ERRORLEVEL 1 EXIT /B 1
call :build_www
IF ERRORLEVEL 1 EXIT /B 1
call :build_mvc
IF ERRORLEVEL 1 EXIT /B 1
call :assemble
IF ERRORLEVEL 1 EXIT /B 1
goto :end


:clean
REM Remove the contents of everything we know we'll be rebuilding
REM This is fairly horrible... we shouldn't need to list it like this, and
REM this isn't really comprehensive.
REM First the Jekyll site... (and latest API docs)
if EXIST %WEB_DIR%\unstable rmdir /s /q %WEB_DIR%\unstable
if EXIST %WEB_DIR%\developer rmdir /s /q %WEB_DIR%\developer
if EXIST %WEB_DIR%\tzdb rmdir /s /q %WEB_DIR%\tzdb
if EXIST %WEB_DIR%\1.0.x\userguide rmdir /s /q %WEB_DIR%\1.0.x\userguide
if EXIST %WEB_DIR%\1.1.x\userguide rmdir /s /q %WEB_DIR%\1.1.x\userguide
if EXIST %WEB_DIR%\1.2.x\userguide rmdir /s /q %WEB_DIR%\1.2.x\userguide
if EXIST %WEB_DIR%\1.3.x\userguide rmdir /s /q %WEB_DIR%\1.3.x\userguide

REM Now the web app...
if EXIST %WEB_DIR%\Content rmdir /s /q %WEB_DIR%\Content
if EXIST %WEB_DIR%\Views rmdir /s /q %WEB_DIR%\Views
if EXIST %WEB_DIR%\Scripts rmdir /s /q %WEB_DIR%\Scripts
if EXIST %WEB_DIR%\bin rmdir /s /q %WEB_DIR%\bin
goto :end


:build_www
pushd ..\www
call jekyll build
IF ERRORLEVEL 1 EXIT /B 1
popd
goto :end


:build_mvc
pushd ..\src\NodaTime.Web
REM Assume NuGet restore has already taken place
REM nuget restore -PackagesDirectory ..\packages
IF ERRORLEVEL 1 EXIT /B 1
REM Build and deploy NodaTime.Web.dll.  See #359 for why we're suppressing PDB
REM generation.
msbuild /t:Rebuild /p:DeployOnBuild=true /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=none
IF ERRORLEVEL 1 EXIT /B 1
popd
goto :end


:assemble
xcopy /Q /E /Y /I ..\www\_site %WEB_DIR%
xcopy /Q /E /Y /I tmp\apidocs %WEB_DIR%\unstable\api
xcopy /Q /E /Y /I ..\src\NodaTime.Web\obj\Release\Package\PackageTmp %WEB_DIR%
goto :end

:end
