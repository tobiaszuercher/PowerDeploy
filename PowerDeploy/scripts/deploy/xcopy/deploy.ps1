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

function Backup()
{
	# do the backkup
	Write-Host "[Backup]"

	if (Test-Path $drop_location)
	{
		$backup_target = ".\Backup_$(Get-Date -Format yyyy-MM-dd_HH.mm.ss)"

		New-Item -Name $backup_target -ItemType directory

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
		Remove-Item $drop_location -Force -Recurse
	}

	New-Item -Path $drop_location -ItemType directory

	Write-Host "unzipping package to $drop_location"

	.\tools\7za.exe x "-o$($drop_location)" ".\package.zip"
}

function ShowHelp()
{
	$actions | Format-Table name,@{ n = 'Description'; e = { $_.Value.Description } }
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

if ($All -eq $false -and $Deploy -eq $false -and $Backup -eq $false)
{
	$package_name = xmlPeek ".\package.xml" "package/@id"
	$package_version = xmlPeek ".\package.xml" "package/@version"
	$package_env = xmlPeek ".\package.xml" "package/@environment"

	Write-Host ""
	Write-Host ""
	Write-Host ("You're about to deploy package {0} v{1} targeting {2}" -f $package_name, $package_version, $package_env.ToUpper())
	Write-Host "" 
	Write-Host "You can do the following commands:"
}
else
{
	$Help = $true
}

$drop_location = xmlPeek ".\package.xml" "/package/droplocation"

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