$xml = [xml](Get-Content (Join-Path (Split-Path -parent $MyInvocation.MyCommand.path) "shell.settings"))

if((get-module | where{$_.name -eq "PowerDeploy"} | Measure-Object).Count -gt 0)
{
    Remove-Module PowerDeploy
}

Import-Module (Join-Path $xml.config.PowerDeployInstallDir "PowerDeploy.psm1") -DisableNameChecking

Write-Host "__________                         ________                .__                "
Write-Host "\______   \______  _  __ __________\______ \   ____ ______ |  |   ____ ___.__."
Write-Host " |     ___/  _ \ \/ \/ // __ \_  __ \    |  \_/ __ \\____ \|  |  /  _ <   |  |"
Write-Host " |    |  (  <_> )     /\  ___/|  | \/    `   \  ___/|  |_> >  |_(  <_> )___  |"
Write-Host " |____|   \____/ \/\_/  \___  >__| /_______  /\___  >   __/|____/\____// ____|"
Write-Host "                            \/             \/     \/|__|               \/     "
Write-Host "                                                             by tobias zürcher"
Write-Host ""
Write-Host "The following commands are available"
Write-Host "  Build {unit-type}"
Write-Host "  Prepare {deploymentUnitSet} {environment}"
Write-Host "  Configure {environment}"
Write-Host "  Deploy {deploymentUnitSet} {environment} TODO"

#TODO: maybe show available packages/environments

Initialize-PowerDeploy $xml.config.ProjectDir

$Host.UI.RawUI.WindowTitle = "PowerDeploy for project in $($xml.config.ProjectDir)"

. .\scripts\prompt.ps1

Set-Alias Build Invoke-Build
Set-Alias Prepare Prepare-DeploymentUnit
Set-Alias Configure Configure-Environment
Set-Alias op Open-ProjectDir