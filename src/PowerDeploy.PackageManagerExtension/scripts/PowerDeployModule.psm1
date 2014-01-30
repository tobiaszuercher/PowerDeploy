$environments = "local", "dev", "test", "prod"

function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0)] [string]$environment
    )

	Get-Project -All | % { Invoke-DirectoryTransform -Environment $environment -Directory (Split-Path -parent $_.Fullname) }
}

function Get-Environments {
	[CmdletBinding()]
	param()

	Get-EnvironmentDir | Get-ChildItem | % { $_.Basename }
}

Register-TabExpansion 'Switch-Environment' @{
    'environment' = $environments
}

Export-ModuleMember Get-Environments
Export-ModuleMember Switch-Environment