function Import-DeploymentUnits()
{
    $result = @{}

    # create folder if not there    
    if ((Test-Path "$($powerdeploy.paths.deployment_unit_configs)") -eq $false)
    {
        New-Item -path "$($powerdeploy.paths.deployment_unit_configs)" -type directory | Out-Null
    }

    # iterate over all deploymentUnit configs and parse them
    foreach($file in Get-ChildItem "$($powerdeploy.paths.deployment_unit_configs)")
    {
        $units = ([xml](Get-Content "$($file.Fullname)")).deploymentunits.deploymentUnit
    
        $result[$file.BaseName] = $units
    }
    
    $powerdeploy.deployment_units = $result;
}