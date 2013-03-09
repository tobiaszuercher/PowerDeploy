Write-Host ""
Write-Host "__________                         ________                .__                "
Write-Host "\______   \______  _  __ __________\______ \   ____ ______ |  |   ____ ___.__."
Write-Host " |     ___/  _ \ \/ \/ // __ \_  __ \    |  \_/ __ \\____ \|  |  /  _ <   |  |"
Write-Host " |    |  (  <_> )     /\  ___/|  | \/    `   \  ___/|  |_> >  |_(  <_> )___  |"
Write-Host " |____|   \____/ \/\_/  \___  >__| /_______  /\___  >   __/|____/\____// ____|"
Write-Host "                            \/             \/     \/|__|               \/     "
Write-Host "                                                            by tobias zuercher"
Write-Host ""
Write-Host "The following commands are available"
Write-Host ""

Set-Location (Split-Path -parent $MyInvocation.MyCommand.path)

Set-Location ..

Set-Alias package .\tools\deploy.ps1

.\tools\deploy.ps1 -Help