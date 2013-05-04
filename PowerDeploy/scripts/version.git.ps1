[CmdletBinding()]
	param()

Push-Location "$($powerdeploy.paths.project)"
$revision = (git rev-list --reverse HEAD).Count
Pop-Location

return $revision