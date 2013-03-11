function Show-PowerDeployHelp()
{
	Write-Host "The following commands are available"
	Write-Host "  Build " -nonewline -f "White"
	Write-Host "    {unit-type}"
	Write-Host "  Configure " -nonewline -f "White"
	Write-Host "{environment}"
	Write-Host "  Prepare " -nonewline -f "White"
	Write-Host "  {deploymentUnitSet} {environment}"
	Write-Host "  Deploy " -nonewline -f "White"
	Write-Host "   {deploymentUnitSet} {environment}"

	Write-Host
	Write-Host "Type the " -nonewline
	Write-Host "command" -f $powerdeploy.colors.strong -nonewline
	Write-Host " without any parameters to get more information about it."
}