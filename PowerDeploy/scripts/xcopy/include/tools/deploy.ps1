[CmdletBinding()]
Param(
    [Parameter(Position = 1)]
    [string]$todo = $null,

    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help
    # TODO: $Rollback
)

#$ErrorActionPreference = "Stop"

# set current directory to package "root" (one up)
$work_dir = Split-Path -parent (Split-Path -parent "$($MyInvocation.MyCommand.Path)")

Push-Location "$work_dir"

Start-Transcript "deploy.txt"

function Backup()
{
	if (Test-Path "$drop_location")
	{
		$backup_target = Join-Path $work_dir "Backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss)"

		Write-Host "Creating backup..."
		Write-Verbose "Target: $backup_target"

		New-Item -Path "$backup_target" -ItemType directory | out-null

		Write-Verbose "copying from $drop_location to $backup_target"
		
		Copy-Item "$drop_location\*" "$backup_target" | out-null
	}
	else
	{
		Write-Host "Skipped backup, nothing found in $drop_location"
	}
}

function DoDeploy()
{
	Write-Host "deploy target: $drop_location"

	if (Test-Path "$drop_location")
	{
		Write-Host "Removing target location ($drop_location)"
		Remove-Item "$drop_location" -Force -Recurse
	}

	New-Item -Path "$drop_location" -ItemType directory | out-null

	Write-Host "deploying package to $drop_location"

	.\tools\7za.exe x "-o$($drop_location)" ".\package.zip" | out-null
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

function xmlPeek($filePath, $xpath)
{ 
    [xml] $fileXml = Get-Content $filePath 
    $found = $fileXml.SelectSingleNode($xpath)

    if ($found.GetType().Name -eq 'XmlAttribute') { return $found.Value }

    return $found.InnerText
} 

function Write-Welcome
{
	$package_name = xmlPeek $package_xml "package/@id"
	$package_version = xmlPeek $package_xml "package/@version"
	$package_env = xmlPeek $package_xml "package/@environment"


	Write-Host ""
	Write-Host ""
	Write-Host "Welcome to your deploy shell!"
	Write-Host ""
	Write-Host "  Package: " -nonewline
	Write-Host $package_name v$package_version -ForegroundColor Red -nonewline
	Write-Host ""
	Write-Host ("           targeting {0}" -f $package_env.ToUpper())
	Write-Host "" 
	Write-Host ""
}

$package_xml = Join-Path "$work_dir" "package.xml"
$drop_location = xmlPeek "$package_xml" "/package/droplocation"

if ($Deploy -eq $false -and $Backup -eq $false -and $Restore -eq $false)
{
	$Help = $true
}

if ($Help)
{
	Write-Welcome
	ShowHelp
}

if ($Backup) { Backup }
if ($Restore) { Restore }
if ($Deploy) 
{
	Backup
	DoDeploy
}

Stop-Transcript

Pop-Location