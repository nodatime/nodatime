@echo off
REM TODO: Don't do this in a batch file!

IF NOT EXIST "%SHFBROOT%\NodaTime.presentation" (
  echo Copy NodaTime.presentation into the SHFB root folder before running buildapidocs.bat
  exit /b 1
)

set SRCDIR=..\src
set DESTDIR=tmp\docs
set DATADIR=..\data
IF NOT EXIST tmp mkdir tmp
IF NOT EXIST %DESTDIR% mkdir %DESTDIR%

REM Really force a clean build...
rmdir /s /q %SRCDIR%\NodaTime\bin\Release
rmdir /s /q %SRCDIR%\NodaTime\bin\"Release Portable"

msbuild Tools.sln /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

REM Get the PCL version ready to build...
BuildProjectVariants\bin\Release\BuildProjectVariants ..\src
IF ERRORLEVEL 1 EXIT /B 1

REM Do the actual builds
msbuild %SRCDIR%\NodaTime-Core.sln /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1
msbuild %SRCDIR%\NodaTime-Core-pcl.sln /property:Configuration=Release
IF ERRORLEVEL 1 EXIT /B 1

set REL=%SRCDIR%\NodaTime\bin\Release
set TESTING_REL=%SRCDIR%\NodaTime.Testing\bin\Release
set JSONNET_REL=%SRCDIR%\NodaTime.Serialization.JsonNet\bin\Release
set ANNOTATOR=DocumentAnnotations\bin\Release\DocumentAnnotations.exe
set VERSION_DOC=DocumentVersions\bin\Release\DocumentVersions
set MREFBUILDER="c:\Program Files (x86)\Sandcastle\ProductionTools\mrefbuilder.exe"

%MREFBUILDER% %REL%\NodaTime.dll /out:%REL%\NodaTime-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1
%MREFBUILDER% %TESTING_REL%\NodaTime.Testing.dll /out:%TESTING_REL%\NodaTime.Testing-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1
%MREFBUILDER% %JSONNET_REL%\NodaTime.Serialization.JsonNet.dll /out:%JSONNET_REL%\NodaTime.Serialization.JsonNet-Ref.xml
IF ERRORLEVEL 1 EXIT /B 1

%ANNOTATOR% %REL%\NodaTime.dll %REL%\NodaTime.xml %REL%\NodaTime.xml
IF ERRORLEVEL 1 EXIT /B 1
%ANNOTATOR% %TESTING_REL%\NodaTime.Testing.dll %TESTING_REL%\NodaTime.Testing.xml %TESTING_REL%\NodaTime.Testing.xml
IF ERRORLEVEL 1 EXIT /B 1
%ANNOTATOR% %JSONNET_REL%\NodaTime.Serialization.JsonNet.dll %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml
IF ERRORLEVEL 1 EXIT /B 1

%VERSION_DOC% %REL%\NodaTime.xml %REL%\NodaTime-Ref.xml "%SRCDIR%\NodaTime\bin\Release Portable\NodaTime.xml" %DATADIR%\versionxml %DESTDIR%\history.txt
IF ERRORLEVEL 1 EXIT /B 1
%VERSION_DOC% %TESTING_REL%\NodaTime.Testing.xml %TESTING_REL%\NodaTime.Testing-Ref.xml "%SRCDIR%\NodaTime.Testing\bin\Release Portable\NodaTime.Testing.xml" %DATADIR%\versionxml %DESTDIR%\history-testing.txt
IF ERRORLEVEL 1 EXIT /B 1
%VERSION_DOC% %JSONNET_REL%\NodaTime.Serialization.JsonNet.xml %JSONNET_REL%\NodaTime.Serialization.JsonNet-Ref.xml "%SRCDIR%\NodaTime.Serialization.JsonNet\bin\Release Portable\NodaTime.Serialization.JsonNet.xml" %DATADIR%\versionxml %DESTDIR%\history-jsonnet.txt
IF ERRORLEVEL 1 EXIT /B 1

REM Prepare the Sandcastle style, by copying then customizing the VS2010 style
REM Note that NodaTime.Presentation refers to %STYLE_DIR%; it must be an absolute path.

set STYLE_DIR=%CD%\tmp\SandcastleStyle
REM TODO: Make this more portable...
set SANDCASTLE="%ProgramFiles(x86)%\Sandcastle"
IF EXIST %STYLE_DIR% rmdir /s /q %STYLE_DIR%
mkdir %STYLE_DIR%
xcopy /E %SANDCASTLE%\Presentation\vs2010 %STYLE_DIR%
mkdir %STYLE_DIR%\SharedContent
copy "%SHFBROOT%"\SharedContent\VS2010BuilderContent_en-US.xml %STYLE_DIR%\SharedContent\NodaTimeBuilderContent_en-US.xml
TweakSandcastleStyle\bin\Release\TweakSandcastleStyle %STYLE_DIR%

msbuild NodaTime.shfbproj
IF ERRORLEVEL 1 EXIT /B 1
