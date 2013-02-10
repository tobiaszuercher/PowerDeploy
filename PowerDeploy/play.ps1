#join-path $scriptPath variables.ps1

#get-help join-path


#. "C:\git\PowerDeploy\PowerDeployvariables.ps1"

#."C:\git\PowerDeploy\PowerDeploy\PowerDeploy.psm1"

#Import-Module PowerDeploy

if((get-module | where{$_.name -eq "PowerDeploy"} | Measure-Object).Count -gt 0)
{
    Remove-Module PowerDeploy
}

Import-Module "C:\git\PowerDeploy\PowerDeploy\PowerDeploy.psm1"
Get-Module

Initialize-PowerDeploy C:\git\PowerDeploy\SampleApp