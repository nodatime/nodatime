@echo off

if "%1" == "" (
  echo Usage: buildweb output-directory
  echo e.g. buildweb c:\users\jon\NodaTime\nodatime.org
  echo It is expected that the output directory already exists and is
  echo set up for git...
  goto end
)

set WEB_DIR=%1

call :build_apidocs
IF ERRORLEVEL 1 EXIT /B 1
call :build_mvc
IF ERRORLEVEL 1 EXIT /B 1
call :clean
IF ERRORLEVEL 1 EXIT /B 1
call :assemble
IF ERRORLEVEL 1 EXIT /B 1
goto :end

:build_apidocs
IF EXIST ..\src\NodaTime.Web\docfx rmdir /s /q ..\src\NodaTime.Web\docfx
pushd ..\docs
call builddocs.bat
IF ERRORLEVEL 1 EXIT /B 1
popd
xcopy /Q /E /Y /I ..\docs\_site ..\src\NodaTime.Web\docfx
goto :end

:build_mvc
IF EXIST ..\src\NodaTime.Web\bin\Release rmdir /s /q ..\src\NodaTime.Web\bin\Release
dotnet restore ..\src\NodaTime.Web
dotnet publish -c Release ..\src\NodaTime.Web
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
xcopy /Q /E /Y /I ..\src\NodaTime.Web\bin\Release\netcoreapp1.0\publish %WEB_DIR%
goto :end

:end
