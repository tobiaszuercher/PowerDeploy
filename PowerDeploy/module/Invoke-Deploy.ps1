function Invoke-Deploy([string]$deploymentUnit, [string]$environment, [string]$subenv = '')
{
	$environment = $environment.ToUpper()

	$deploy_script = "$($powerdeploy.paths.deployment_units)/$deploymentUnit/$environment$subenv/bulkdeploy.ps1"

	if (Test-Path $deploy_script)
	{
		# preparation: if one package needs admin, would be nice to start the script in admin mode (with UAC request)
		#if (@(Get-ChildItem .\dbserver -Recurse -Filter NEED_ADMIN_TO_DEPLOY).length -eq 0)
		#{
			& "$deploy_script" -Deploy
		#}
	}
    else
    {
    	Write-Host "There is no package for $deploymentUnit targeting environment $environment. Please build and prepare it first!" -Foreground "Red"
	}
}