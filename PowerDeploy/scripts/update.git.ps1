# TODO: read git-executable from shell.settings

if((Test-Path $powerdeploy.config.PowerDeployInstallDir) -eq $false)
{
	New-Item -Path $powerdeploy.config.PowerDeployInstallDir -ItemType directory | Out-Null

	Write-Host "No PowerDeploy installation found. I'll install it for you, stay tuned!"

	git clone $powerdeploy.project.update.repository $powerdeploy.project.update.target
}
else
{
	Write-Host "updating PowerDeploy"

	$location = Get-Location

	Set-Location $powerdeploy.config.PowerDeployInstallDir

	git fetch # todo: do pulling, but i'm in testmode now :)

	Set-Location $location	
}