function Invoke-Deploy([string]$deploymentUnit, [string]$environment)
{
	$environment = $environment.ToUpper()

	$path = "$($powerdeploy.paths.deployment_units)/$deploymentUnit/$environment"

	if (Test-Path $path)
	{
		Invoke-Expression -Command "$path\bulkdeploy.ps1 -Deploy"
	}
    else
    {
    	Write-Host "There is no package for $deploymentUnit targeting environment $environment" -Foreground "Red"
	}
}