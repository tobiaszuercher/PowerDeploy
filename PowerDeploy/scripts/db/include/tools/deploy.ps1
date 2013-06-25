[CmdletBinding()]
Param(
    [Parameter(Position = 1)]
    [string]$file,

    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help,
    [switch] $Rollback,
    [switch] $CloseOnEnd
)

#$ErrorActionPreference = "Stop"

# package is one folder up...
$package_dir = Resolve-Path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

. (Join-Path "$package_dir" "tools\deploy.common.ps1")

Push-Location "$package_dir"

function Backup($server, $name)
{
	Write-Host "Creat backup for database $name from $server"

	$backup_file_name = "backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss).bak"

	# well it's really lazy to grant everyone and full, but we'll close the share after the export
	cmd.exe /C $("net share pdbackupshare=`"{0}`" /GRANT:Everyone,Full" -f "$package_dir")

	$backup_share = "\\$env:computername\pdbackupshare\$backup_file_name"
	sqlcmd -S "$server" -i "scripts/backup.sql" -v DatabaseName="$name" BackupFile="$backup_share"

	Get-WmiObject -Class Win32_Share -Filter "Name='pdbackupshare'" | Remove-WmiObject

	return $backup_file_name
}

function Rollback()
{
	if ($file -eq '')
	{
		Write-Host "Following backups are available:"
		Write-Host 

		Get-ChildItem -filter backup_* | Format-table Name -HideTableHeaders

		Write-Host "Usage example: " -nonewline
		Write-Host "  package -Rollback backup_2000-01-12_13:37.bak" -f Red
		Write-Host
	}
	else
	{
		DoRollback $file
	}
}

function DoRollback($file)
{
	Write-Host "Start rollback..."

	if ($file.StartsWith('\\'))
	{
		$backup_share_path = $file
	}
	else
	{
		cmd.exe /C $("net share pdRollbackShare=`"{0}`" /GRANT:Everyone,Full" -f "$package_dir")
		$backup_share_path = "\\$env:computername\pdRollbackShare\$file"
	}

	DropDatabase

	sqlcmd -S "$dbserver" -i "scripts/restore.sql" -v DatabaseName="$dbname" BackupFile="$backup_share_path"


	Get-WmiObject -Class Win32_Share -Filter "Name='pdRollbackShare'" | Remove-WmiObject

	Write-Host "Rollback completed!"
}

function DropDatabase
{
	Write-Host "Dropping $dbname..."

	sqlcmd -S "$dbserver" -i "scripts/kill.connections.sql" -v DatabaseName="$dbname"
	sqlcmd -S "$dbserver" -i "scripts/drop.sql" -v DatabaseName="$dbname"
}

function DoDeploy()
{
	Write-Host "deploy to $dbname on $dbserver"

	.\tools\7za.exe x "-oscripts" -y ".\package.zip" | out-null

	# if unc data deployment enabled: first restore backup file from unc path
	if($datadeploy.StartsWith("unc"))
	{
		Write-Verbose "Restore from unc"

		$file = $datadeploy.Split(":")[1]

		if ($file -eq $null)
		{
			throw "no path for unc data deployment defined!"
		}

		DoRollback $file
	}
	elseif ($datadeploy.StartsWith("livedump"))
	{
		# livedump:dbname@dbserver\instance
		$target = $datadeploy.Split(":")[1].Split("@")

		Write-Verbose "Livedump from db" $target[0] "on server" $target[1]

		$live_backup = Backup $target[1] $target[0]
		DoRollback $live_backup
	}
	else
	{
		# if no db was imported then drop the database explicitly
		DropDatabase
	}

	Write-Host "Executing migration scripts"

	Get-ChildItem ".\scripts/Migrations" -Filter *.sql -Recurse | % {
		Write-Host "executing $($_.Name)"

		if ($_.Name.Contains("CreateDatabase")) # maybe think for a better solution... if db doesn't exist, we cannt call sqlcmd with a "-d not-existing-db-name"
		{
			sqlcmd -S "$dbserver" -i "$($_.Fullname)" -v DatabaseName="$dbname"
		}
		else
		{
			sqlcmd -S "$dbserver" -i "$($_.Fullname)" -v DatabaseName="$dbname" -d "$dbname"
		}
	}

	# execute data scripts for datascripts if datadeploy is datascripts
	if ($datadeploy.StartsWith("datascripts"))
	{
		$datascripts = $datadeploy.Split(":")[1]

		if ($datascripts -eq $null)
		{
			$datascripts = "default"
		}

		Write-Host "Executing scripts for datascripts $datascripts"

		Get-ChildItem ".\scripts/DataScripts/$datascripts" -Filter *.sql -Recurse | % {
			Write-Host "executing $($_.Name)"
			
			sqlcmd -S "$dbserver" -i "$($_.Fullname)" -v DatabaseName="$dbname" -d "$dbname"
		}		
	}
}

function ShowHelp()
{
	Write-Host " Use package -command where command is one of the following:"
	Write-Host "   -Deploy	  Deploys $package_name to $package_appserver$package_virtualdir"
	Write-Host "   -Backup	  Backubps the currently deployed $package_name on $package_appserver."
	Write-Host "   -Rollback	  Rollbacks a previously created backup."
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

$package_xml = Join-Path $package_dir "package.xml"
$dbserver = xmlPeek $package_xml "/package/dbserver"
$dbname = xmlPeek $package_xml "/package/dbname"
$datadeploy = xmlPeek $package_xml "/package/datadeploy"

if ($Deploy -eq $false -and $Backup -eq $false -and $Rollback -eq $false)
{
	$Help = $true
}

if ($Help)
{
	Write-Welcome
	Write-PackageInfo
	ShowHelp
}

if ($Backup) { Backup $dbserver $dbname }
if ($Rollback) { Rollback }
if ($Deploy) 
{
	Backup $dbserver $dbname
	DoDeploy
}

Pop-Location


Start-Sleep -s 5

if ($CloseOnEnd)
{
	Stop-Process -Id $PID
}