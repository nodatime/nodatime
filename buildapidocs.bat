@echo off
REM TODO: Don't do this in a batch file!

IF NOT EXIST "%SHFBROOT%\NodaTime.presentation" (
  echo Copy NodaTime.presentation into the SHFB root folder before running buildapidocs.bat
  exit /b 1
)

REM Really force a clean build...
rmdir /s /q src\NodaTime\bin\Release
rmdir /s /q src\NodaTime\bin\"Release Portable"

msbuild "src\NodaTime-All.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1
msbuild "src\NodaTime-All.sln" /property:Configuration="Release Portable"
IF ERRORLEVEL 1 EXIT /B 1
set REL=src\NodaTime\bin\Release
"c:\Program Files (x86)\Sandcastle\ProductionTools\mrefbuilder.exe" %REL%\NodaTime.dll /out:%REL%\NodaTime-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1

src\NodaTime.Tools.AnnotationDocumentor\bin\Release\NodaTime.Tools.AnnotationDocumentor.exe %REL%\NodaTime.dll %REL%\NodaTime.xml %REL%\NodaTime.xml
IF ERRORLEVEL 1 EXIT /B 1
src\NodaTime.Tools.VersionDocumentor\bin\Release\NodaTime.Tools.VersionDocumentor %REL%\NodaTime.xml %REL%\NodaTime-Ref.xml "src\NodaTime\bin\Release Portable\NodaTime.xml" data\versionxml docs\history.txt
IF ERRORLEVEL 1 EXIT /B 1

REM Prepare the Sandcastle style, by copying then customizing the VS2010 style
set STYLE_DIR=%CD%\SandcastleStyleTmp
REM TODO: Make this more portable...
set SANDCASTLE="%ProgramFiles(x86)%\Sandcastle"
IF EXIST %STYLE_DIR% rmdir /s /q %STYLE_DIR%
mkdir %STYLE_DIR%
xcopy /E %SANDCASTLE%\Presentation\vs2010 %STYLE_DIR%
mkdir %STYLE_DIR%\SharedContent
copy "%SHFBROOT%"\SharedContent\VS2010BuilderContent_en-US.xml %STYLE_DIR%\SharedContent\NodaTimeBuilderContent_en-US.xml
src\NodaTime.Tools.SandcastleStyleTweaker\bin\Release\NodaTime.Tools.SandcastleStyleTweaker %STYLE_DIR%


msbuild NodaTime.shfbproj
IF ERRORLEVEL 1 EXIT /B 1
