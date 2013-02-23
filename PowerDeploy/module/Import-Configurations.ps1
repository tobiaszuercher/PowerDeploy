function Import-Configurations
{
    $powerdeploy.project = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/project.xml"))).project
    $powerdeploy.packages = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/neutralpackages.xml"))).neutralpackages.package
}