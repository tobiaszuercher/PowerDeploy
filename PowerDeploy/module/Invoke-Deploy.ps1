function Invoke-Deploy([string]$deploymentUnit, [string]$environment, [string]$subenv = '')
{
	$environment = $environment.ToUpper()

	$path = "$($powerdeploy.paths.deployment_units)/$deploymentUnit/$environment$subenv"

	if (Test-Path $path)
	{
		Invoke-Expression -Command "$path\bulkdeploy.ps1 -Deploy"
	}
    else
    {
    	Write-Host "There is no package for $deploymentUnit targeting environment $environment" -Foreground "Red"
	}
}