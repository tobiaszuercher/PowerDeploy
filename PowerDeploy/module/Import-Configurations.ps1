function Import-Configurations
{
    $powerdeploy.project = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/project.xml") -Encoding UTF8)).project
    $powerdeploy.packages = ([xml] (Get-Content (Join-Path $powerdeploy.paths.project "configuration/neutralpackages.xml") -Encoding UTF8)).neutralpackages.package
}