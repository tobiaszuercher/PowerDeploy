function Invoke-Build([string]$type)
{
	# Make sure we have the latest configurations
	Import-Configurations
	Import-DeploymentUnits

    foreach ($package in $powerdeploy.packages | where { $_.type -eq $type })
    {
        $project_file = Join-Path (Join-Path $powerdeploy.paths.project "/implementation/source/") $package.source
        Invoke-Expression -Command "$(Join-Path $($powerdeploy.paths.scripts) build.$type.ps1) $project_file $($package.id) $($package.configPrefix) -Build -Package"
    }
}