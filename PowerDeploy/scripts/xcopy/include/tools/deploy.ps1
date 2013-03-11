[CmdletBinding()]
Param(
    [Parameter(Position = 1)]
    [string]$todo = $null,

    # todo: i'm not sure whtere to do swtiches
    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help
)

$ErrorActionPreference = "Stop"

# package is one folder up... wtf!!! isn't there an easier way to do that?
$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

Push-Location $work_dir

function Backup()
{
	if (Test-Path $drop_location)
	{
		$backup_target = Join-Path $work_dir "Backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss)"

		Write-Host "Creating backup..."
		Write-Verbose "Target: $backup_target"

		New-Item -Path $backup_target -ItemType directory | out-null

		Write-Verbose "copying from $drop_location to $backup_target"
		
		Copy-Item "$drop_location\*" $backup_target | out-null
	}
	else
	{
		Write-Host "Skipped backup, nothing found in $drop_location"
	}
}

function DoDeploy()
{
	Write-Host "deploy target: $drop_location"
	Write-Host "removing $drop_location"

	if (Test-Path $drop_location)
	{
		Write-Host "Removing target location ($drop_location)"
		Remove-Item $drop_location -Force -Recurse
	}

	New-Item -Path $drop_location -ItemType directory | out-null

	Write-Host "deploying package to $drop_location"

	.\tools\7za.exe x "-o$($drop_location)" ".\package.zip" | out-null
}

function ShowHelp()
{
	#$actions | Format-Table name,@{ n = 'Description'; e = { $_.Value.Description } } -AutoSize

	Write-Host "Deploy "
	Write-Host "Backup"
	Write-Host "Help"
	Write-Host ""
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

	cls

	Write-Host ""
	Write-Host ""
	Write-Host "Welcome to your deploy shell!"
	Write-Host ""
	Write-Host "  Package: " -nonewline
	Write-Host $package_name v$package_version -ForegroundColor Red -nonewline
	Write-Host ""
	Write-Host ("          targeting {0}" -f $package_env.ToUpper())
	Write-Host "" 
	Write-Host ""
}

# i'm still unsure which approach seems to be better... use the [switch]-parameters or "task1,task2,task3" approach
# i let this hashtable here to get ShowHelp() for free.

$package_xml = Join-Path $work_dir "package.xml"
$drop_location = xmlPeek $package_xml "/package/droplocation"

if ($All -eq $false -and $Deploy -eq $false -and $Backup -eq $false)
{
	Write-Welcome
}
else
{
	$Help = $true
}

if ($Help) { ShowHelp }
if ($Backup) { Backup }
if ($Deploy) 
{
	Backup
	DoDeploy
}

Pop-Location