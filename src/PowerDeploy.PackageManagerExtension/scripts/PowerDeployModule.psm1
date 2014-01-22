$environments = "local", "dev", "test", "prod"

#pwd | write-host -> C:\git\PowerDeploy\src

function Switch-Environment($environment) {
	Get-Module

	Get-Project -All | % { Invoke-DirectoryTransform -Environment $environment -Directory (Split-Path -parent $_.Fullname) }
	
	#Add-Type -Path (Join-Path "lib/net451" "PowerDeploy.PackageManagerExtension.dll")
	#[PowerDeploy.PackageManagerExtension.PowerShellBuddy]::ConfigureEnvironment("bla", $null)
}

Register-TabExpansion 'Switch-Environment' @{
    'environment' = { "local", "dev", "test", "prod" }
}

Export-ModuleMember Switch-Environment