[CmdletBinding()]
param()

$version_seed_file = Join-Path $powerdeploy.paths.project $powerdeploy.project.versioning.seed

Write-Verbose "using version seed file: $version_seed_file"

$version_seed = [int] (Get-Content $version_seed_file)

if ($version_seed -eq $null)
{
	Write-Verbose "version seed file was empty or not existing"
	$version_seed = 0
}

$version_seed += 1

Set-Content $version_seed_file $version_seed

return $version_seed