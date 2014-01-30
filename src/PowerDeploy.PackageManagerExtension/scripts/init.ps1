param($installPath, $toolsPath, $package)

Import-Module (Join-Path $toolsPath "PowerDeploy.PackageManagerExtension.dll")
Import-Module (Join-Path $toolsPath PowerDeployModule.psm1)