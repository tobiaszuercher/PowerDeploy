function Get-Version()
{
	$type = $powerdeploy.project.versioning.type

	# run powerdeploy/scripts/version.git.ps1
	$revision = & $(Join-Path "$($powerdeploy.paths.scripts)" version.$type.ps1)
	
	return $powerdeploy.project.assembly.version -replace '\*', $revision
}