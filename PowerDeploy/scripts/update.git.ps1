# TODO: read git-executable from project.xml

if((Test-Path $powerdeploy.project.update.target) -eq $false)
{
	New-Item -Path $powerdeploy.project.update.target -ItemType directory | Out-Null

	Write-Host "No PowerDeploy installation found. Install one, stay tuned!"

	git clone $powerdeploy.project.update.repository $powerdeploy.project.update.target
}
else
{
	Write-Host "updating PowerDeploy"

	$location = Get-Location

	Set-Location $powerdeploy.project.update.target

	git pull

	Set-Location $location	
}