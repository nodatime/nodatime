@echo off

pushd src

for %%s in (*.sln) DO (
  echo Restoring %%s packages
  nuget restore %%s
)

popd src
