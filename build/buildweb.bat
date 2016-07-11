@echo off

if "%1" == "" (
  echo Usage: buildweb output-directory
  echo e.g. buildweb c:\users\jon\NodaTime\nodatime.org
  echo It is expected that the output directory already exists and is
  echo set up for git...
  goto end
)

set WEB_DIR=%1

call buildapidocs
IF ERRORLEVEL 1 EXIT /B 1
call :build_www
IF ERRORLEVEL 1 EXIT /B 1
call :build_mvc
IF ERRORLEVEL 1 EXIT /B 1
call :fetch-fixed-content
IF ERRORLEVEL 1 EXIT /B 1
call :clean
IF ERRORLEVEL 1 EXIT /B 1
call :assemble
IF ERRORLEVEL 1 EXIT /B 1
goto :end


:build_www
pushd ..\www
call jekyll build
IF ERRORLEVEL 1 EXIT /B 1
popd
goto :end


:build_mvc
dotnet publish -c Release ..\src\NodaTime.Web
IF ERRORLEVEL 1 EXIT /B 1
goto :end

:fetch-fixed-content
IF EXIST tmp\fixed-content rmdir /s /q tmp\fixed-content
git clone --depth 1 -b fixed-content https://github.com/nodatime/nodatime.org.git tmp\fixed-content
rmdir /s /q tmp\fixed-content\.git
IF ERRORLEVEL 1 EXIT /B 1
goto :end

:clean
REM Retain just the .git directory, but nuke the rest from orbit.
IF EXIST tmp\old_nodatime.org rmdir /s /q tmp\old_nodatime.org
move %WEB_DIR% tmp\old_nodatime.org
mkdir %WEB_DIR%
attrib -h tmp\old_nodatime.org\.git
move tmp\old_nodatime.org\.git %WEB_DIR%
attrib +h %WEB_DIR%\.git
goto :end


:assemble
xcopy /Q /E /Y /I ..\www\_site %WEB_DIR%\wwwroot
xcopy /Q /E /Y /I tmp\fixed-content %WEB_DIR%
xcopy /Q /E /Y /I tmp\apidocs %WEB_DIR%\wwwroot\unstable\api
xcopy /Q /E /Y /I ..\src\NodaTime.Web\bin\Release\netcoreapp1.0\publish %WEB_DIR%
goto :end

:end
