function Configure-Environment
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0)] [string]$environment,
        [Parameter(Position = 1)] [string]$target_path = $powerdeploy.paths.project, 
        [Parameter(Position = 2)] [boolean]$delete_templates = $false
    )
    
#    process
#    {
        $environment = $environment.ToUpper()

        if (CheckWheterEnvironmentExist $environment)
        {
            Write-Host "Configuring environment for" $environment.ToUpper() " in $target_path" -ForegroundColor "DarkCyan"
           
            foreach($template in Get-ChildItem -Path $target_path -Filter "*.template.*" -Recurse)
            {
                Transform-Template $template.Fullname $environment
                
                if ($delete_templates -eq $true)
                {
                    Remove-Item $template.Fullname -Force
                }
            }
        }
        else
        {
            cls

            if($environment.Length -gt 0)
            {
                Write-Host "Environment " -nonewline
                Write-Host $environment -f Cyan -nonewline
                Write-Host " not found."
            }

            Write-Host ""
            Write-Host "Usage: Configure {environment}    (alias for Configure: config, c"
                        
            Write-Host ""
            Write-Host "The following environments are available:"
            Write-Host ""

            Show-Environments
        }
#    }
}