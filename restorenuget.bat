@echo off

REM TODO: detect these automatically. Probably a job for PowerShell...

set PROJECTS=Benchmarks,CodeDiagnostics,CodeDiagnostics.Test,Demo
set PROJECTS=%PROJECTS%,Serialization.JsonNet,Serialization.Test,Test
set PROJECTS=%PROJECTS%,TzdbCompiler.Test,Web

for %%p in (%PROJECTS%) DO (
  echo Restoring NodaTime.%%p packages
  nuget restore -NonInteractive -PackagesDirectory src\packages src\NodaTime.%%p\packages.config
)
