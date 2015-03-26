@echo off
REM This is really hacky. We should look into using psake...
if not exist "..\NodaTime Release.snk" (
    echo Cannot build NuGet package without NodaTime Release.snk
    goto :end
)

set SRCDIR=..\src
set PUBLICAPIDIR=tmp\PublicApi
set PUBLICPCLAPIDIR=tmp\PublicPclApi

REM This will also build the unsigned release build. A bit wasteful,
REM but that's okay... After this, we will have the project variants created.
call buildapidocs.bat
IF ERRORLEVEL 1 EXIT /B 1

REM Also build the PCL docs...
msbuild NodaTime-Pcl.shfbproj
IF ERRORLEVEL 1 EXIT /B 1

msbuild "%SRCDIR%\NodaTime-Core-signed.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild "%SRCDIR%\NodaTime-Core-signed-pcl.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild "%SRCDIR%\NodaTime-Core-signed-net4.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

copy /y %PUBLICAPIDIR%\NodaTime.xml "%SRCDIR%\NodaTime\bin\Signed Release"
copy /y %PUBLICAPIDIR%\NodaTime.Testing.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release"
copy /y %PUBLICAPIDIR%\NodaTime.Serialization.JsonNet.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release"

copy /y %PUBLICAPIDIR%\NodaTime.xml "%SRCDIR%\NodaTime\bin\Signed Release Net4"
copy /y %PUBLICAPIDIR%\NodaTime.Testing.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release Net4"
copy /y %PUBLICAPIDIR%\NodaTime.Serialization.JsonNet.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release Net4"

copy /y %PUBLICPCLAPIDIR%\PublicPclApi\NodaTime.xml "%SRCDIR%\NodaTime\bin\Signed Release Portable"
copy /y %PUBLICPCLAPIDIR%\PublicPclApi\NodaTime.Testing.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release Portable"
copy /y %PUBLICPCLAPIDIR%\PublicPclApi\NodaTime.Serialization.JsonNet.xml "%SRCDIR%\NodaTime.Testing\bin\Signed Release Portable"

IF EXIST tmp\nuget rmdir /s /q tmp\nuget
mkdir tmp\nuget
pushd tmp\nuget
nuget pack ..\..\%SRCDIR%\NodaTime\NodaTime.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

nuget pack ..\..\%SRCDIR%\NodaTime.Testing\NodaTime.Testing.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

nuget pack ..\..\%SRCDIR%\NodaTime.Serialization.JsonNet\NodaTime.Serialization.JsonNet.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

popd

:end
