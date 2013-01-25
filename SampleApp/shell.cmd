@echo off
cls

set PROJECT_ROOT=%~dp0

color F0
title getting latest version of shared scripts

set ProgFiles86Root=%ProgramFiles(x86)%
if not "%ProgFiles86Root%"=="" goto 64bit
set ProgFiles86Root=%ProgramFiles%
:64bit

call "%ProgFiles86Root%\Microsoft Visual Studio 10.0\VC\bin\vcvars32.bat" > nul

echo getting latest version of shared scripts --^> please stand by...

set SCRIPTSPATH=$/C14821/NeutralPackagesFramework
set TARGET=C:\NeutralPackagesFramework

cd /d c:\
if not exist %TARGET% (
  mkdir %TARGET%
  cd /d %TARGET%
  tf workfold /unmap %TARGET% > nul
  tf workspace /new /server:http://zrhtfs02:8080/tfs/projects /noprompt NP
  tf workfold %SCRIPTSPATH% %TARGET% /workspace:NP
)
cd /d %TARGET%
tf get %TARGET% /recursive /overwrite /noprompt

if "%RUNCOMMAND%"=="" set RUNCOMMAND=%TARGET%/shell.cmd
%RUNCOMMAND%
