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
}

if ($Deploy -eq $true)
{
	# TODO: duplicate detection: just deploy newest

	# loop through each package and execute deploy.ps1 -BulkDeploy
	Get-ChildItem $this_dir | where { $_.PsIsContainer } | ForEach-Object {
		Push-Location $_.Fullname

		Start-Process powershell -ArgumentList "-NoExit -Command $($_.Fullname)\tools\deploy.ps1 -Deploy"

		Pop-Location
	}
}