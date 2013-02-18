function Initialize-PowerDeploy([string]$path = (Split-Path -parent $MyInvocation.MyCommand.path))
{
    $path = Resolve-Path $path # resolve path allows us to have relative paths or .

    # set all paths to the project
    $powerdeploy.paths.project = $path
    $powerdeploy.paths.deployment_units = Join-Path $path "deployment/deploymentUnits"
    $powerdeploy.paths.deployment_unit_configs = Join-Path $path "deployment/deploymentUnitConfigs"
    $powerdeploy.paths.project_config_file = Join-Path $path "configuration/project.xml"

    Import-Configurations
    Import-DeploymentUnits
    
    Set-Alias sz "$($powerdeploy.paths.tools)\7Zip\7za.exe" -Scope "Script"
    
    # todo: code von psake "ausleihen"... 
    # or: http://bradwilson.typepad.com/blog/2010/03/vsvars2010ps1.html
    $env:path = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319" + ";$env:path"

    # todo: maybe print out what was loaded, list of env & deployment unit sets...
    # let the user see that something hapened!
}

function Import-Configurations
{
    $powerdeploy.project = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/project.xml"))).project
    $powerdeploy.packages = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/neutralpackages.xml"))).neutralpackages.package
}