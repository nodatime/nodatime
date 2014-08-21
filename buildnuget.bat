@echo off
REM This is really hacky. We should look into using psake...
if not exist "NodaTime Release.snk" (
    echo Cannot build NuGet package without NodaTime Release.snk
    goto :end
)

REM Make sure we can build all the variants
msbuild "src\NodaTime-Tools.sln" /property:Configuration=Release
src\NodaTime.Tools.ProjectBuilder\bin\Release\NodaTime.Tools.ProjectBuilder src
IF ERRORLEVEL 1 EXIT /B 1

msbuild "src\NodaTime-Core-signed.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild "src\NodaTime-Core-signed-pcl.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

msbuild "src\NodaTime-Core-signed-net4.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

REM This will also build the unsigned release build. A bit wasteful,
REM but that's okay...
call buildapidocs.bat
IF ERRORLEVEL 1 EXIT /B 1

copy /y docs\PublicApi\NodaTime.xml src\NodaTime\bin\Release
copy /y docs\PublicApi\NodaTime.Testing.xml src\NodaTime.Testing\bin\Release
copy /y docs\PublicApi\NodaTime.Serialization.JsonNet.xml src\NodaTime.Testing\bin\Release

rmdir /s /q nuget
mkdir nuget
pushd nuget
nuget pack ..\src\NodaTime\NodaTime.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

nuget pack ..\src\NodaTime.Testing\NodaTime.Testing.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

nuget pack ..\src\NodaTime.Serialization.JsonNet\NodaTime.Serialization.JsonNet.nuspec -Symbols
IF ERRORLEVEL 1 EXIT /B 1

popd

:end
