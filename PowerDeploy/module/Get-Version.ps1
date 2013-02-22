function Get-Version()
{
	$type = $powerdeploy.project.versioning.type

	$revision = Invoke-Expression -Command "$(Join-Path $($powerdeploy.paths.scripts) version.$type.ps1)"
	
	return $powerdeploy.project.assembly.version -replace '\*', $revision
}