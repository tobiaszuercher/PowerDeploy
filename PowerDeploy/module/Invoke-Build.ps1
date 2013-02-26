function Invoke-Build([string]$type)
{
	# Make sure we have the latest configurations
	Import-Configurations
	Import-DeploymentUnits

	$version = Get-Version
    
    $assembly_info = @{
                        AssemblyVersion = $version; 
                        AssemblyFileVersion = $version; 
                        AssemblyInformationalVersion = "Version $version built on $(Get-Date -Format 'dd.MM.yyyy HH:mm')"
                    }

    Update-AssemblyInfo $assembly_info $powerdeploy.paths.project

    Write-Host "Version $version built on $(Get-Date -Format 'dd.MM.yyyy HH:mm')" -ForegroundColor "Blue"

    foreach ($package in $powerdeploy.packages | where { $_.type -eq $type -or $type.ToUpper() -eq "ALL" })
    {
        $project_file = Join-Path (Join-Path $powerdeploy.paths.project "/implementation/source/") $package.source

        # remove if there are some older version of this neutral package
        Get-Childitem $powerdeploy.paths.deployment_units -Filter "$($package.id)*" | Remove-Item -Force -Recurse

        Invoke-Expression -Command "$(Join-Path $($powerdeploy.paths.scripts) build.$($package.type).ps1) $project_file $($package.id) $($package.configPrefix) -Build -Package -Version $version"
    }
}