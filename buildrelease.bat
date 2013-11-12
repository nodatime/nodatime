@echo off

if "%1" == "" (
  echo Usage: buildrelease version-number
  goto end
)

if NOT EXIST "NodaTime Release.snk" (
  echo Copy NodaTime Release.snk into this directory first.
  goto end
)

set VERSION=%1

hg up -r '%VERSION%'
IF ERRORLEVEL 1 EXIT /B 1

hg archive -r '%VERSION%' NodaTime-%VERSION%-src.zip
IF ERRORLEVEL 1 EXIT /B 1

set STAGING=NodaTime-%VERSION%
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
copy "src\NodaTime\bin\Signed Release\NodaTime.dll" %STAGING%
copy "src\NodaTime.Serialization.JsonNet\bin\Signed Release\NodaTime.Serialization.JsonNet.dll" %STAGING%
copy "src\NodaTime.Testing\bin\Signed Release\NodaTime.Testing.dll" %STAGING%
copy "src\NodaTime\bin\Signed Release Portable\NodaTime.dll" %STAGING%\Portable
copy "src\NodaTime.Serialization.JsonNet\bin\Signed Release Portable\NodaTime.Serialization.JsonNet.dll" %STAGING%\Portable
copy "src\NodaTime.Testing\bin\Signed Release Portable\NodaTime.Testing.dll" %STAGING%\Portable
  
zip -r -9 %STAGING%.zip %STAGING%
  
:end
