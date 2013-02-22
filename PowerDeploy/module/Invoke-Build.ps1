function Invoke-Build([string]$type)
{
	# Make sure we have the latest configurations
	Import-Configurations
	Import-DeploymentUnits

	$version = Get-Version

    foreach ($package in $powerdeploy.packages | where { $_.type -eq $type })
    {
        $project_file = Join-Path (Join-Path $powerdeploy.paths.project "/implementation/source/") $package.source

        # remove if there are some older version of this neutral package
        Get-Childitem $powerdeploy.paths.deployment_units -Filter "$($package.id)*" | Remove-Item -Force -Recurse

        Invoke-Expression -Command "$(Join-Path $($powerdeploy.paths.scripts) build.$type.ps1) $project_file $($package.id) $($package.configPrefix) -Build -Package -Version $version"
    }
}