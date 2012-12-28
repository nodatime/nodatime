@echo off
REM This is really hacky. We should look into using psake...
if not exist "NodaTime Release.snk" (
    echo Cannot build NuGet package without NodaTime Release.snk
    goto :end
)

msbuild "src\NodaTime.sln" /property:Configuration="Signed Release"
msbuild "src\NodaTime.sln" /property:Configuration="Signed Release Portable"

REM This will also build the unsigned release build. A bit wasteful,
REM but that's okay...
call builddocs.bat

copy /y docs\PublicApi\NodaTime.xml src\NodaTime\bin\Release
copy /y docs\PublicApi\NodaTime.Testing.xml src\NodaTime.Testing\bin\Release

rmdir /s /q nuget
mkdir nuget
pushd nuget
nuget pack ..\src\NodaTime\NodaTime.nuspec
nuget pack ..\src\NodaTime.Testing\NodaTime.Testing.nuspec
popd

:end
