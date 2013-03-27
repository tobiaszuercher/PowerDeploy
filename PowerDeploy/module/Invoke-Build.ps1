function Invoke-Build([string]$type)
{
    # Make sure we have the latest configurations
    Import-Configurations
    Import-DeploymentUnits

    if ($type -eq '') # haha, its not $null :D
    {
        cls

        Write-Host "The " -nonewline
        Write-Host "Build" -nonewline -f $powerdeploy.colors.strong
        Write-Host " command builds your project(s) into neutral package(s). A neutral package never contains any"
        Write-Host "environment specific information. Use the " -nonewline
        Write-Host "Prepare " -f $powerdeploy.colors.strong -nonewline
        Write-Host "command to apply environment specific informations like"
        Write-Host "connection strings or other typcal configuration values."
        Write-Host
        Write-Host "To switch your local workspace to a specific enviornment, use the " -nonewline
        Write-Host "Config" -f $powerdeploy.colors.strong -nonewline
        Write-Host " command."
        Write-Host
        Write-Host "The neutral packages are located at: $($powerdeploy.paths.deployment_units)"
        Write-Host
        Write-Host "Usage: " -nonewline
        Write-Host "Build " -f $powerdeploy.colors.strong -nonewline
        Write-Host "<package-type>"
        Write-Host "       Where <package-type> is one of all, xcopy or iis"
        Write-Host 

        return
    }

    if ($type.ToUpper() -eq 'CLEAN')
    {
        Write-Host "Cleaning" $powerdeploy.paths.deployment_units
        Get-ChildItem $powerdeploy.paths.deployment_units | Remove-Item -Force -Recurse

        return
    }

	$version = Get-Version
    
    $assembly_info = @{
                        AssemblyVersion = $version; 
                        AssemblyFileVersion = $version; 
                        AssemblyInformationalVersion = ("Version $version built on $(Get-Date -Format 'dd.MM.yyyy HH:mm')");
                        AssemblyCopyright = $ExecutionContext.InvokeCommand.ExpandString($powerdeploy.project.assembly.copyright.ToString());
                        AssemblyCompany = $ExecutionContext.InvokeCommand.ExpandString($powerdeploy.project.assembly.company);
                    }

    Update-AssemblyInfo $assembly_info $powerdeploy.paths.project

    Write-Host "building version $version..." -ForegroundColor "Blue"

    foreach ($package in $powerdeploy.packages | where { $_.type -eq $type -or $type.ToUpper() -eq "ALL" })
    {
        $project_file = Join-Path (Join-Path $powerdeploy.paths.project "/implementation/source/") $package.source

        # remove if there are some older version of this neutral package
        Get-Childitem $powerdeploy.paths.deployment_units -Filter "$($package.id)*" | Remove-Item -Force -Recurse

        Invoke-Expression -Command "$(Join-Path $($powerdeploy.paths.scripts) /$($package.type)/build.$($package.type).ps1) $project_file $($package.id) $($package.configPrefix) -Build -Package -Version $version"
    }
}