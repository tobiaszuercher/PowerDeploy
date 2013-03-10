[CmdletBinding()]
param(
	[switch] $Deploy,
	[switch] $LocalCopyFirst # TODO	
)

$this_dir = (Split-Path -parent $MyInvocation.MyCommand.Definition)

Set-Alias packages .\bulkdeploy.ps1 -Scope Global -Force 

if ($Deploy -eq $false)
{
	cls

	Write-Host "Found the following deployment units to deploy:"

	Get-ChildItem $this_dir | where { $_.PsIsContainer } | ForEach-Object {
		Write-Host " ->" $_.Name
	}

	Write-Host ""
	Write-Host "In order to deploy type " -nonewline
	Write-Host "packages -Deploy" -ForegroundColor "Yellow"
	Write-Host ""

	Set-Alias packages ".\$($MyInvocation.MyCommand.Name)" -Scope Global
	Set-Location $this_dir
}

if ($Deploy -eq $true)
{
	$location_backup = Get-Location

	# TODO: duplicate detection: just deploy newest

	# loop through each package and execute deploy.ps1 -BulkDeploy
	Get-ChildItem $this_dir | where { $_.PsIsContainer } | ForEach-Object {
		Set-Location $_

		.\tools\deploy.ps1 -Deploy
	}

	Set-Location $location_backup
}