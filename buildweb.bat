@echo off

if "%2" == "" (
  echo Usage: buildweb stable-web-docs-directory output-directory
  echo e.g. buildweb c:\users\jon\StableWebDocs c:\users\jon\nodatime.org
  echo It is expected that the output directory already exists and is
  echo set up for git...
  goto end
)

set STABLE_DIR=%1
set WEB_DIR=%2

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
IF EXIST tmpwebdir rmdir /s /q tmpwebdir

REM Hacky way to start with a clean directory
move %WEB_DIR% tmpwebdir
mkdir %WEB_DIR%
attrib -h tmpwebdir\.git
move tmpwebdir\.git %WEB_DIR%
attrib +h %WEB_DIR%\.git
goto :end


:build_www
pushd www
call jekyll build
IF ERRORLEVEL 1 EXIT /B 1
popd
goto :end


:build_mvc
pushd src\NodaTime.Web
nuget restore -PackagesDirectory ..\packages
IF ERRORLEVEL 1 EXIT /B 1
msbuild /t:Rebuild /p:DeployOnBuild=true /p:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1
popd
goto :end


:assemble
xcopy /Q /E /Y www\_site %WEB_DIR%
xcopy /Q /E /Y /I docs\api %WEB_DIR%\unstable\api
xcopy /Q /E /Y %STABLE_DIR% %WEB_DIR%
xcopy /Q /E /Y src\NodaTime.Web\obj\Release\Package\PackageTmp %WEB_DIR%
goto :end

:end
