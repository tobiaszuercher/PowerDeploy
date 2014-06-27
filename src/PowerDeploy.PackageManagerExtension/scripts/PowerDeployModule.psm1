function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0)] [string]$environment,
		[Parameter()] [string]$PasswordFile,
		[Parameter()] [string]$Password
    )
	Write-Host "Transform templates for each project-folder in the current solution..."

	Get-Project -All | Where-Object  { $_.Type -ne 'Web Site' } | % { Invoke-DirectoryTransform -Environment $environment -Directory (Split-Path -parent $_.Fullname) -PasswordFile $PasswordFile -Password $Password }
}

function Get-Environments {
	[CmdletBinding()]
	param()

	Get-EnvironmentDir | Get-ChildItem | % { $_.Basename }
}

function Create-EncryptionKey {
	[CmdletBinding()]
	param(
		[Parameter] [string]$PasswordFile
	)

	Invoke-CreateEncryptionKey -PasswordFile $PasswordFile
}

function Encrypt-Environments {
	[CmdletBinding()]
	param(
		[Parameter()] [string]$PasswordFile,
		[Parameter()] [string]$Password
	)

	Write-Host "Encrypting all variables..."

	Invoke-EncryptEnvironments -Directory $pwd -PasswordFile $PasswordFile -Password $Password
}

Export-ModuleMember Get-Environments
Export-ModuleMember Switch-Environment
Export-ModuleMember Encrypt-Environments
Export-ModuleMember Create-EncryptionKey

#Register-TabExpansion 'Switch-Environment' @{ 'environment' = { Get-EnvironmentDir | Get-ChildItem | % { $_.Basename } } }