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

$actions = @{
	deploy = @{ command = "deploy"; description = "Deploys the xcopy backage to the specified drop location" };
	help = @{ command = "help"; description = "Prints this informations out." };
	backup = @{ command = "backup"; description = "Backups the current drop location" }
}

function Backup()
{
	Write-Error "Backup not implemented for IIS atm :("
}

function DoDeploy()
{
	Write-Host "Deploying iis package to web site $package_website into virtual directory $package_virtualdir"
	Write-Host "removing $drop_location"

	if (Test-Path $drop_location)
	{
		Write-Host "Removing target location ($drop_location)"
		Remove-Item $drop_location -Force -Recurse
	}

	New-Item -Path $drop_location -ItemType directory

	Write-Host "unzipping package to $drop_location"

	Import-Module .\tools\PowerDeploy.Extensions.dll

	Create-AppPool -Name $package_apppoolname -ServerName $package_appserver -WAMUserName $package_username -WAMUserPass -package_password
	Assign-AppPool -ServerName $package_appserver -AppPoolName $package_apppoolname -VirtualDirectory $package_virtualdir -WebsiteName $package_website

	#.\tools\7za.exe x "-o$($drop_location)" ".\package.zip"
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
	Write-Host ("   targeting {0}" -f $package_env.ToUpper())
	Write-Host "" 
	Write-Host ""
}

$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

$package_xml = Join-Path $work_dir "package.xml"
$package_appserver = xmlPeek $package_xml "/package/appserver"
$package_username = xmlPeek $package_xml "/package/username"
$package_password = xmlPeek $package_xml "/package/password"
$package_apppoolname = xmlPeek $package_xml "/package/apppoolname"
$package_virtualdir = xmlPeek $package_xml "/package/virtualdir"
$package_website = xmlPeek $package_xml "/package/website"

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