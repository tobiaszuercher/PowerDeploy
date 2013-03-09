title loading powerdeploy

echo loading power deploy shell...

cd "%~dp0"
powershell.exe -noexit -command ". %~dp0tools\deploy.shell.ps1"