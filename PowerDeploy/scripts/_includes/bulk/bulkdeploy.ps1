[CmdletBinding()]
param(
	[switch] $Deploy,
	[switch] $LocalCopyFirst # TODO: if on unc path, its "praktisch" to make a local copy first
)

$script_dir = (Split-Path -parent "$($MyInvocation.MyCommand.Definition)")

Push-Location "$script_dir"

Set-Alias packages .\bulkdeploy.ps1 -Scope Global -Force 

if ($Deploy -eq $false)
{
	cls

	Write-Host "Found the following deployment units to deploy:"

	Get-ChildItem "$script_dir" | where { $_.PsIsContainer } | ForEach-Object {
		Write-Host " ->" "$($_.Name)"
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
	# TODO: wait for the results

	Write-Host ""
	Write-Host "Start deployment..."
	Write-Host ""

	Get-ChildItem "$script_dir" | where { $_.PsIsContainer } | ForEach-Object {
		Push-Location "$($_.Fullname)"

		if (Test-Path .\tools\NEED_ADMIN_TO_DEPLOY)
		{
			Write-Host " -> deploy $($_.Name)"
			Start-Process powershell -ArgumentList "-Command .\tools\deploy.ps1 -Deploy" -Verb runas
		}
		else
		{
			Write-Host " -> deploy $($_.Name)"
			Start-Process powershell -ArgumentList "-Command .\tools\deploy.ps1 -Deploy"
		}

		Pop-Location
	}

	Write-Host ""
	Write-Host "...please wait till all subwindows are closed."
	Write-Host ""
}

Pop-Location