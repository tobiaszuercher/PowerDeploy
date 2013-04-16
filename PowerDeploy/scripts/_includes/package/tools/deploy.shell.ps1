cls

Write-Host ""
Write-Host "__________                         ________                .__                "
Write-Host "\______   \______  _  __ __________\______ \   ____ ______ |  |   ____ ___.__."
Write-Host " |     ___/  _ \ \/ \/ // __ \_  __ \    |  \_/ __ \\____ \|  |  /  _ <   |  |"
Write-Host " |    |  (  <_> )     /\  ___/|  | \/    `   \  ___/|  |_> >  |_(  <_> )___  |"
Write-Host " |____|   \____/ \/\_/  \___  >__| /_______  /\___  >   __/|____/\____// ____|"
Write-Host "                            \/             \/     \/|__|               \/     "
Write-Host "                                                            by tobias zuercher"

Set-Location (Split-Path -parent $MyInvocation.MyCommand.path)

Set-Location ..

Set-Alias package .\tools\deploy.ps1

.\tools\deploy.ps1 -Help

#Write-Host Black      -f Black       
#Write-Host Blue       -f Blue      
#Write-Host Cyan       -f Cyan       
#Write-Host DarkBlue   -f DarkBlue    
#Write-Host DarkCyan   -f DarkCyan    
#Write-Host DarkGray   -f DarkGray    
#Write-Host DarkGreen  -f DarkGreen   
#Write-Host DarkMagenta -f DarkMagenta
#Write-Host DarkRed    -f DarkRed    
#Write-Host DarkYellow -f DarkYellow  
#Write-Host Gray       -f Gray        
#Write-Host Green      -f Green      
#Write-Host Magenta    -f Magenta     
#Write-Host Red        -f Red         
#Write-Host White      -f White      
#Write-Host Yellow     -f Yellow      