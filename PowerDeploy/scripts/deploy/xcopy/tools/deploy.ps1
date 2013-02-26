[CmdletBinding()]
Param(
    [Parameter(Position = 1)]
    [string]$todo = $null,

    # todo: i'm not sure whtere to do swtiches
    [switch] $Deploy,
    [switch] $Backup,
    [switch] $All,
    [switch] $Help
)

$ErrorActionPreference = "Stop"

$actions = @{
	deploy = @{ command = "deploy"; description = "Deploys the xcopy backage to the specified drop location" };
	help = @{ command = "help"; description = "Prints this informations out." };
	backup = @{ command = "backup"; description = "Backups the current drop location" }
}

# package is one folder up... wtf!!! isn't there an easier way to do that?
$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

function Backup()
{
	# do the backkup
	Write-Host "[Backup]"

	if (Test-Path $drop_location)
	{
		$backup_target = Join-Path $work_dir "Backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss)"

		Write-Verbose "Create backup target: $backup_target"

		New-Item -Path $backup_target -ItemType directory

		Write-Verbose "copying from $drop_location to $backup_target"
		Copy-Item "$drop_location\*" $backup_target
	}
	else
	{
		Write-Host "Nothing found in $drop_location"
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

	New-Item -Path $drop_location -ItemType directory

	Write-Host "unzipping package to $drop_location"

	.\tools\7za.exe x "-o$($drop_location)" ".\package.zip"
}

function ShowHelp()
{
	$actions | Format-Table name,@{ n = 'Description'; e = { $_.Value.Description } } -AutoSize
}

function xmlPeek($filePath, $xpath)
{ 
    [xml] $fileXml = Get-Content $filePath 
    $found = $fileXml.SelectSingleNode($xpath)

    if ($found.GetType().Name -eq 'XmlAttribute') { return $found.Value }

    return $found.InnerText
} 

# i'm still unsure which approach seems to be better... use the [switch]-parameters or "task1,task2,task3" approach
# i let this hashtable here to get ShowHelp() for free.

$package_xml = Join-Path $work_dir "package.xml"
$drop_location = xmlPeek $package_xml "/package/droplocation"

if ($All -eq $false -and $Deploy -eq $false -and $Backup -eq $false)
{
	$package_name = xmlPeek $package_xml "package/@id"
	$package_version = xmlPeek $package_xml "package/@version"
	$package_env = xmlPeek $package_xml "package/@environment"

	Write-Host ""
	Write-Host ""
	Write-Host "You're about to deploy package " -nonewline
	Write-Host $package_name -ForegroundColor Red -nonewline
	Write-Host (" v{1} targeting {2}" -f $package_name, $package_version, $package_env.ToUpper())
	Write-Host "" 
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

# pretty funny way to use on command line deploy.ps1 "task1,task2,task3:"

#$actions = @{
#	deploy = @{ action = { DoDeploy }; description = "Deploys the xcopy backage to the specified drop location" };
#	backup = @{ action = { Backup }; description = "Backups the current drop location" }
#	help = @{ action = { ShowHelp }; description = "Prints this informations out." }
#}

#if ($todo -eq $null -or $todo.length -eq 0 )
#{
#	ShowHelp
#}
#else
#{
#	$todo.split(',') | % { & $actions.$_.action }
#}