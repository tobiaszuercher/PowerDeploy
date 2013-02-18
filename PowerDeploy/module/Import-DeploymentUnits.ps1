function Import-DeploymentUnits()
{
    $result = @{}
    
    # iterate over all deploymentUnit configs and parse them
    foreach($file in Get-ChildItem $powerdeploy.paths.deployment_unit_configs)
    {
        $units = ([xml](get-content $file.Fullname)).deploymentunits.deploymentUnit
    
        $result[$file.BaseName] = $units
    }
    
    $powerdeploy.deployment_units = $result;
}