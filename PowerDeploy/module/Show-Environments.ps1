function Show-Environments
{
	$env_path = Join-Path $powerdeploy.paths.environments $powerdeploy.project.id

    Get-ChildItem $env_path -Filter "*.xml" | ForEach-Object {
        $env_name = " - {0}" -f $_.Basename
        $env_description = "`t({0})" -f (xmlPeek $_.Fullname "/environment/@description")

        Write-Host $env_name -nonewline -ForegroundColor White
        
    	if ($env_description.Length -gt 4)
    	{
        	Write-Host $env_description
    	}
    	else
    	{
    		Write-Host ""
    	}
    }

    Write-Host ""
}