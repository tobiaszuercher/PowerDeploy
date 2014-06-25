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

function Create-EncryptionKey {
	[CmdletBinding()]
	param(
		[Parameter(Position = 0)] [string]$passwordFile
	)

	Invoke-CreateEncryptionKey -PasswordFile $passwordFile
}

function Encrypt-Environments {
	[CmdletBinding()]
	param(
		[Parameter(Position = 0)] [string]$passwordFile
	)
	Write-Host "Encrypting all variables..."

	Invoke-EncryptEnvironments -Directory $pwd -PasswordFile $passwordFile
}

Export-ModuleMember Get-Environments
Export-ModuleMember Switch-Environment
Export-ModuleMember Encrypt-Environments
Export-ModuleMember Create-EncryptionKey

Register-TabExpansion 'Switch-Environment' @{ 'environment' = { Get-EnvironmentDir | Get-ChildItem | % { $_.Basename } } }