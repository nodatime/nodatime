@echo off

REM First in the build directory...
for %%s in (*.sln) DO (
  echo Restoring %%s packages
  nuget restore %%s
)

REM Then in the src directory...
pushd ..\src

for %%s in (*.sln) DO (
  echo Restoring %%s packages
  nuget restore %%s
)

popd
