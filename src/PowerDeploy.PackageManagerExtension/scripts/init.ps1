param($installPath, $toolsPath, $package)

#Add-Type -Path (Join-Path (Join-Path $installPath "lib/net451") "PowerDeploy.PackageManagerExtension.dll")

Import-Module (Join-Path $toolsPath "PowerDeploy.PackageManagerExtension.dll")
Import-Module (Join-Path $toolsPath PowerDeployModule.psm1)