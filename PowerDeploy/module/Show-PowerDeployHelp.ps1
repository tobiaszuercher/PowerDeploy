function Show-PowerDeployHelp()
{
	Write-Host "The following commands are available"
	Write-Host "  Build " -nonewline -f "White"
	Write-Host "{unit-type}"
	Write-Host "  Prepare " -nonewline -f "White"
	Write-Host "{deploymentUnitSet} {environment}"
	Write-Host "  Configure " -nonewline -f "White"
	Write-Host "{environment}"
	Write-Host "  Deploy " -nonewline -f "White"
	Write-Host "{deploymentUnitSet} {environment}"
}