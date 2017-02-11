@echo off

REM This could be in the build directory instead, but let's keep it
REM here for the minute...

set PREVIOUS_VERSIONS=1.0.x 1.1.x 1.2.x 1.3.x

echo Fetching previous versions from source control if necessary
if not exist history (
  mkdir history
  for %%V in (%PREVIOUS_VERSIONS%) do (
    echo Cloning %%V
    git clone https://github.com/nodatime/nodatime.git -q --depth 1 -b %%V history\%%V || goto error
  )
) else (
  echo Directory for previous versions already exists.
  echo Checking we have all the versions we need...
  for %%V in (%PREVIOUS_VERSIONS%) do (
    if not exist history\%%V (
      echo Previous version %%V has not been cloned.
      echo Please delete the history directory and rerun.
      goto error
    )
  )
  echo Looks good.
)

REM Temporary docfx output directory
if exist obj rmdir /s /q obj
mkdir obj

REM Final docfx output directory
if exist _site rmdir /s /q _site
if exist api rmdir /s /q api

for %%V in (%PREVIOUS_VERSIONS%) do (
  echo Building metadata for %%V
  copy /y docfx-csproj.json history\%%V\docfx.json
  if exist history\%%V\api rmdir /s /q history\%%V\api
  call docfx history\%%V\docfx.json metadata -f || goto error
  xcopy /I /S /Q history\%%V\api obj\%%V\api
  copy toc.yml obj\%%V
)

echo Building metadata for current branch
call docfx metadata -f
copy toc.yml obj\unstable

REM TODO: Add extra information (versions etc)

echo Running main docfx build
call docfx build

goto end

:error
echo Build failed.
exit /B 1

:end
