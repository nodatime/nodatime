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

REM Get the PCL version ready to build...
src\NodaTime.Tools.ProjectBuilder\bin\Release\NodaTime.Tools.ProjectBuilder src
IF ERRORLEVEL 1 EXIT /B 1

msbuild "src\NodaTime-Core-pcl.sln" /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

set REL=src\NodaTime\bin\Release
set TESTING_REL=src\NodaTime.Testing\bin\Release
set JSONNET_REL=src\NodaTime.Serialization.JsonNet\bin\Release
set ANNOTATOR=src\NodaTime.Tools.AnnotationDocumentor\bin\Release\NodaTime.Tools.AnnotationDocumentor.exe
set VERSION_DOC=src\NodaTime.Tools.VersionDocumentor\bin\Release\NodaTime.Tools.VersionDocumentor

"c:\Program Files (x86)\Sandcastle\ProductionTools\mrefbuilder.exe" %REL%\NodaTime.dll /out:%REL%\NodaTime-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1
"c:\Program Files (x86)\Sandcastle\ProductionTools\mrefbuilder.exe" %TESTING_REL%\NodaTime.Testing.dll /out:%TESTING_REL%\NodaTime.Testing-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1
"c:\Program Files (x86)\Sandcastle\ProductionTools\mrefbuilder.exe" %JSONNET_REL%\NodaTime.Serialization.JsonNet.dll /out:%JSONNET_REL%\NodaTime.Serialization.JsonNet-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1

%ANNOTATOR% %REL%\NodaTime.dll %REL%\NodaTime.xml %REL%\NodaTime.xml
IF ERRORLEVEL 1 EXIT /B 1
%ANNOTATOR% %TESTING_REL%\NodaTime.Testing.dll %TESTING_REL%\NodaTime.Testing.xml %TESTING_REL%\NodaTime.Testing.xml
IF ERRORLEVEL 1 EXIT /B 1
%ANNOTATOR% %JSONNET_REL%\NodaTime.Serialization.JsonNet.dll %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml
IF ERRORLEVEL 1 EXIT /B 1

%VERSION_DOC% %REL%\NodaTime.xml %REL%\NodaTime-Ref.xml "src\NodaTime\bin\Release Portable\NodaTime.xml" data\versionxml docs\history.txt
IF ERRORLEVEL 1 EXIT /B 1
%VERSION_DOC% %TESTING_REL%\NodaTime.Testing.xml %TESTING_REL%\NodaTime.Testing-Ref.xml "src\NodaTime.Testing\bin\Release Portable\NodaTime.Testing.xml" data\versionxml docs\history-testing.txt
IF ERRORLEVEL 1 EXIT /B 1
%VERSION_DOC% %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml %JSONNET_REL%\NodaTime.Serialization.JsonNet-Ref.xml "src\NodaTime.Serialization.JsonNet\bin\Release Portable\NodaTime.Serialization.JsonNet.xml" data\versionxml docs\history-jsonnet.txt
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
