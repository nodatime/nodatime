REM TODO: Don't do this in a batch file!

REM Really force a clean build...
rmdir /s /q src\NodaTime\bin\Release

msbuild "src\NodaTime.sln" /property:Configuration=Release
msbuild "tools\NodaTime.Tools.sln" /property:Configuration=Release
tools\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs tools\userguide-src docs\userguide
tools\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs tools\developer-src docs\developer
msbuild NodaTime.shfbproj
