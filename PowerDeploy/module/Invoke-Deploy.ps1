function Invoke-Deploy([string]$deploymentUnit, [string]$environment, [string]$subenv = '')
{
	$environment = $environment.ToUpper()

	$path = "$($powerdeploy.paths.deployment_units)/$deploymentUnit/$environment$subenv"

	if (Test-Path $path)
	{
		# preparation: if one package needs admin, would be nice to start the script in admin mode (with UAC request)
		#if (@(Get-ChildItem .\dbserver -Recurse -Filter NEED_ADMIN_TO_DEPLOY).length -eq 0)
		#{
			Invoke-Expression -Command "$path\bulkdeploy.ps1 -Deploy"
		#}
	}
    else
    {
    	Write-Host "There is no package for $deploymentUnit targeting environment $environment" -Foreground "Red"
	}
}