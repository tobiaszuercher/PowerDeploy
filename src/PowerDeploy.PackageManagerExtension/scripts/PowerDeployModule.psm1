function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0)] [string]$environment
    )
	Write-Host "Transform templates for each project-folder in the current solution..."

	Get-Project -All | Where-Object  { $_.Type -ne 'Web Site' } | % { Invoke-DirectoryTransform -Environment $environment -Directory (Split-Path -parent $_.Fullname) }
}

function Get-Environments {
	[CmdletBinding()]
	param()

	Get-EnvironmentDir | Get-ChildItem | % { $_.Basename }
}

Export-ModuleMember Get-Environments
Export-ModuleMember Switch-Environment

Register-TabExpansion 'Switch-Environment' @{ 'environment' = { Get-EnvironmentDir | Get-ChildItem | % { $_.Basename } } }