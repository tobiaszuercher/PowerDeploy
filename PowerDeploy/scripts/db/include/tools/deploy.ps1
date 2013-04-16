[CmdletBinding()]
Param(
    [Parameter(Position = 1)]
    [string]$backup_file = $null,

    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help,
    [switch] $Restore
)

$ErrorActionPreference = "Stop"

. .\tools\deploy.common.ps1

# package is one folder up...
$work_dir = Resolve-Path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

Push-Location $work_dir

function Backup()
{
	$backup_file_name = "Backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss).bak"

	cmd.exe /C $("net share pdbackupshare=`"{0}`" /GRANT:Everyone,Full" -f "$work_dir")

	$backup_share = "\\$env:computername\pdbackupshare\$backup_file_name"
	sqlcmd -S "$dbserver" -i "scripts/backup.sql" -v DatabaseName="$dbname" BackupFile="$backup_share"

	Get-WmiObject -Class Win32_Share -Filter "Name='pdbackupshare'" | Remove-WmiObject
}

function Restore()
{
	Write-Host "Dropping $dbname"
	sqlcmd -S "$dbserver" -i "scripts/drop.sql" -v DatabaseName="$dbname"

	cmd.exe /C $("net share pdrestoreshare=`"{0}`" /GRANT:Everyone,Full" -f "$work_dir")

	$backup_share = "\\$env:computername\pdrestoreshare\$backup_file"

	sqlcmd -S "$dbserver" -i "scripts/kill.connections.sql" -v DatabaseName="$dbname"
	sqlcmd -S "$dbserver" -i "scripts/restore.sql" -v DatabaseName="$dbname" BackupFile="$backup_share"

	Get-WmiObject -Class Win32_Share -Filter "Name='pdrestoreshare'" | Remove-WmiObject
}

function DoDeploy()
{
	Write-Host "deploy to $dbname on $dbserver"

	.\tools\7za.exe x "-oscripts" ".\package.zip" | out-null

	Get-ChildItem ".\scripts/Migrations" -Filter *.sql -Recurse | % {
		Write-Host "executing $($_.Name)"

		if ($_.Name.Contains("CreateDatabase")) # maybe think for a better solution...
		{
			sqlcmd -S "$dbserver" -i "$($_.Fullname)" -v DatabaseName="$dbname"
		}
		else
		{
			sqlcmd -S "$dbserver" -i "$($_.Fullname)" -v DatabaseName="$dbname" -d "$dbname"
		}
	}
}

function ShowHelp()
{
	Write-Host " Use package -command where command is one of the following:"
	Write-Host "   -Deploy	  Deploys $package_name to $package_appserver$package_virtualdir"
	Write-Host "   -Backup	  Backups the currently deployed $package_name on $package_appserver."
	Write-Host "   -Restore	  Restores a previously created backup."
	Write-Host "   -Help 	  Shows help information."
	Write-Host
}

function Write-PackageInfo
{
	Write-Host "  Package Info:"
	Write-Host "    db name:   " -nonewline
	Write-Host "$dbname" -ForegroundColor Yellow
	Write-Host "    db server: " -nonewline
	Write-Host "$dbserver" -ForegroundColor Yellow
	Write-Host ""
}

$package_xml = Join-Path $work_dir "package.xml"
$dbserver = xmlPeek $package_xml "/package/dbserver"
$dbname = xmlPeek $package_xml /"package/dbname"

if ($Deploy -eq $false -and $Backup -eq $false -and $Restore -eq $false)
{
	$Help = $true
}

if ($Help)
{
	Write-Welcome
	Write-PackageInfo
	ShowHelp
}

if ($Backup) { Backup }
if ($Restore) { Restore }
if ($Deploy) 
{
	Backup
	DoDeploy
}

Pop-Location