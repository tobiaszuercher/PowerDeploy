function Write-PowerDeployStatus
{
	Write-Host " " -nonewline
	Write-Host "[" -ForegroundColor "Yellow" -nonewline

	if ($powerdeploy.project.id -eq $false)
	{
		Write-Host "powerdeploy not initialized" -nonewline -ForegroundColor "Blue"
	}
	else
	{
		Write-Host $powerdeploy.project.id -ForegroundColor "Blue" -nonewline
	}

	Write-Host "]" -ForegroundColor "Yellow" -nonewline	
}

function prompt {
    $realLASTEXITCODE = $LASTEXITCODE

    Write-Host($pwd) -nonewline

    Write-PowerDeployStatus

    $global:LASTEXITCODE = $realLASTEXITCODE
    return "> "
}