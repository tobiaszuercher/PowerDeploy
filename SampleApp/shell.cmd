@echo off

title loading powerdeploy

echo loading power deploy shell...

cd "%~dp0"
powershell.exe -noexit -command ". %~dp0shell.ps1"