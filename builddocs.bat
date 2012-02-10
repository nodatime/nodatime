REM TODO: Don't do this in a batch file!

msbuild "src\NodaTime VS2010.sln" /property:Configuration=Release
msbuild "tools\NodaTime.Tools.sln" /property:Configuration=Release
tools\NodaTime.Tools.BuildMarkdownDocs\bin\Release\NodaTime.Tools.BuildMarkdownDocs tools\userguide-src docs\userguide
REM msbuild NodaTime.shfbproj
