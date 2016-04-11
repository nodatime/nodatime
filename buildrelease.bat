@echo off

if "%1" == "" (
  echo Usage: buildrelease version-number
  goto end
)

if NOT EXIST "NodaTime Release.snk" (
  echo Copy NodaTime Release.snk into the root directory first.
  goto end
)

set VERSION=%1
set SRCDIR=src

IF EXIST tmp rmdir /s /q tmp
mkdir tmp

REM Note: this means we end up at a detached head. Don't try to commit any changes
REM without checking out the branch again.
git checkout %VERSION%
IF ERRORLEVEL 1 EXIT /B 1

git archive %VERSION% -o NodaTime-%VERSION%-src.zip --prefix=NodaTime-%VERSION%-src/

set STAGING=tmp\NodaTime-%VERSION%
IF EXIST %STAGING% rmdir /s /q %STAGING%
mkdir %STAGING%
mkdir %STAGING%\docs

call buildofflineguide.bat %1

call buildnuget
IF ERRORLEVEL 1 EXIT /B 1

xcopy /q /s /i docs\api %STAGING%\docs\api
xcopy /q /s /i tmpdocs\_site %STAGING%\docs\userguide
copy AUTHORS.txt %STAGING%
copy LICENSE.txt %STAGING%
copy NOTICE.txt %STAGING%
copy "NodaTime Release Public Key.snk" %STAGING%
copy readme.txt %STAGING%

mkdir %STAGING%\Portable
copy docs\PublicApi\NodaTime.xml %STAGING%
copy docs\PublicApi\NodaTime.Testing.xml %STAGING%
copy docs\PublicApi\NodaTime.Serialization.JsonNet.xml %STAGING%
copy "%SRCDIR%\NodaTime\bin\Signed Release\NodaTime.dll" %STAGING%
copy "%SRCDIR%\NodaTime.Serialization.JsonNet\bin\Signed Release\NodaTime.Serialization.JsonNet.dll" %STAGING%
copy "%SRCDIR%\NodaTime.Testing\bin\Signed Release\NodaTime.Testing.dll" %STAGING%
copy "%SRCDIR%\NodaTime\bin\Signed Release Portable\NodaTime.dll" %STAGING%\Portable
copy "%SRCDIR%\NodaTime.Serialization.JsonNet\bin\Signed Release Portable\NodaTime.Serialization.JsonNet.dll" %STAGING%\Portable
copy "%SRCDIR%\NodaTime.Testing\bin\Signed Release Portable\NodaTime.Testing.dll" %STAGING%\Portable

cd tmp
zip -r -9 NodaTime-%VERSION%.zip NodaTime-%VERSION%
cd ..

:end
