[CmdletBinding()]
param(
	[switch] $Deploy,
	[switch] $LocalCopyFirst
)

$this_dir = (Split-Path -parent $MyInvocation.MyCommand.Definition)

if ($Deploy -eq $false)
{
	Write-Host "Found the following deployment units to deploy:"
	Get-ChildItem | ForEach-Object { # todo: just folder
		Write-Host " ->" $_.Name
	}

	Write-Host ""
	Write-Host "In order to deploy type " -nonewline
	Write-Host "packages -Deploy" -ForegroundColor "Yellow"

	Set-Alias packages ".\$($MyInvocation.MyCommand.Name)" -Scope Global
}

if ($Deploy -eq $true)
{
	$location_backup = Get-Location

	# loop through each package and execute deploy.ps1 -BulkDeploy
	Get-ChildItem | where { $_.PsIsContainer } | ForEach-Object {
		Set-Location $_

		.\tools\deploy.ps1 -Deploy
	}

	Set-Location $location_backup
}