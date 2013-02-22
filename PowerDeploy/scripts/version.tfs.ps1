[CmdletBinding()]
param()

if((Get-Module | where{$_.name -eq "PowerDeploy.Extensions"} | Measure-Object).Count -gt 0)
{
	Remove-Module "PowerDeploy.Extensions"
}

Import-Module (Join-Path $powerdeploy.paths.root_dir "module/TFS/PowerDeploy.Extensions.dll")

return Get-TfsChangesetNumber $powerdeploy.project.versioning.path $powerdeploy.project.versioning.tfsserver