clear

$xml = [xml](Get-Content (Join-Path (Split-Path -parent $MyInvocation.MyCommand.path) "shell.settings"))

if((get-module | where{$_.name -eq "PowerDeploy"} | Measure-Object).Count -gt 0)
{
    Remove-Module PowerDeploy
}

Import-Module (Join-Path $xml.config.PowerDeployInstallDir "PowerDeploy.psm1") -DisableNameChecking

$powerdeploy.config = $xml.config

Write-Host "__________                         ________                .__                "
Write-Host "\______   \______  _  __ __________\______ \   ____ ______ |  |   ____ ___.__."
Write-Host " |     ___/  _ \ \/ \/ // __ \_  __ \    |  \_/ __ \\____ \|  |  /  _ <   |  |"
Write-Host " |    |  (  <_> )     /\  ___/|  | \/    `   \  ___/|  |_> >  |_(  <_> )___  |"
Write-Host " |____|   \____/ \/\_/  \___  >__| /_______  /\___  >   __/|____/\____// ____|"
Write-Host "                            \/             \/     \/|__|               \/     "
Write-Host "                                                             by tobias zürcher"
Write-Host ""

Show-PowerDeployHelp

Write-Host ""

#TODO: maybe show available packages/environments

Initialize-PowerDeploy $xml.config.ProjectDir

$Host.UI.RawUI.WindowTitle = "PowerDeploy for project " + $powerdeploy.project.id + " in $($xml.config.ProjectDir)"

# todo make nice prompt
#. .\scripts\prompt.ps1

Set-Alias Build Invoke-Build -Scope Global
Set-Alias Prepare Prepare-DeploymentUnit -Scope Global
Set-Alias Configure Configure-Enviornment -Scope Global
Set-Alias Config Configure-Enviornment -Scope Global
Set-Alias op Open-ProjectDir -Scope Global
Set-Alias Help Show-PowerDeployHelp -Scope Global