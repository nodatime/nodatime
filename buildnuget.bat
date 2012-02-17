@echo off
REM This is really hacky. We should look into using psake...

REM This will build the release build as well
call builddocs.bat

copy /y docs\PublicApi\NodaTime.xml src\NodaTime\bin\Release
copy /y docs\PublicApi\NodaTime.Testing.xml src\NodaTime.Testing\bin\Release

rmdir /s /q nuget
mkdir nuget
pushd nuget
nuget pack ..\src\NodaTime\NodaTime.nuspec
nuget pack ..\src\NodaTime.Testing\NodaTime.Testing.nuspec
popd
