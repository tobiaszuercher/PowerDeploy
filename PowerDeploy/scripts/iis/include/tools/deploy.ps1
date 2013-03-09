[CmdletBinding()]
Param(
    [Parameter()] [string] $backup_package = '',
    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help,
    [switch] $RestoreBackup,
    [switch] $UpdateExisting
)

$ErrorActionPreference = "Stop"

$actions = @{
	deploy = @{ command = "deploy"; description = "Deploys the xcopy backage to the specified drop location" };
	help = @{ command = "help"; description = "Prints this informations out." };
	backup = @{ command = "backup"; description = "Backups the current drop location" }
}

function Backup()
{
	Write-Host "Create backup for $package_appserver/$package_virtualdir"

	Import-Module .\tools\PowerDeploy.Extensions.dll -DisableNameChecking

	# WHAT THE FUCK, LOOK AT THOSE `! this IS ridiculous!
	.\tools\MsDeploy\x64\msdeploy.exe -source:contentPath=`'Default Web Site/SampleAppWeb`' -dest:"package='backup_$(Get-Date -Format yyyy-MM-dd_HH-mm-ss).zip'" -verb:sync
}

function RestoreBackup()
{
	cls

	if ($backup_package -eq '')
	{
		Write-Host "Following backups available:"
		Write-Host 

		Get-ChildItem -filter backup_* | Format-table Name -HideTableHeaders

		Write-Host "Usage example: " -nonewline
		Write-Host "TODO -RestoreBackup backup_2000-01-12_13:37.zip" -f Red
		Write-Host
	}
	else
	{
		Write-Host "Restoring package $backup_package"
		Write-Host 

		# remove optional .\ in front of filename (just if the juster pressed Format-table)
		DoDeploy (Get-Item $backup_package).Fullname
	}
}

function DoDeploy([string] $package = "package.zip")
{
	Write-Host "Deploying iis package to web site $package_website into virtual directory $package_virtualdir"

	Import-Module .\tools\PowerDeploy.Extensions.dll -DisableNameChecking

	# todo get-architecture
	.\tools\MsDeploy\x64\msdeploy.exe -source:package="$package" -dest:"auto,includeAcls='False',computerName='$package_appserver',authType='NTLM'" -verb:sync -disableLink:ContentExtension -disableLink:CertificateExtension -allowUntrusted

	Write-Host

	Create-AppPool -Name $package_apppoolname -ServerName $package_appserver -WAMUserName $package_username -WAMUserPass -package_password
	Assign-AppPool -ServerName $package_appserver -ApplicationPoolName $package_apppoolname -VirtualDirectory $package_virtualdir -WebsiteName $package_website

	#.\tools\7za.exe x "-o$($drop_location)" ".\package.zip"

	Write-Host "Done! have fun!"
}

function ShowHelp()
{
	#$actions | Format-Table name,@{ n = 'Description'; e = { $_.Value.Description } } -AutoSize

	Write-Welcome

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



if ($All -eq $false -and $Deploy -eq $false -and $Backup -eq $false -and $RestoreBackup -eq $false)
{
	Write-Welcome

	$Help = $true
}

if ($Help) { ShowHelp }
if ($Backup) { Backup }
if ($RestoreBackup) { RestoreBackup }
if ($Deploy) 
{
	#Backup
	DoDeploy
}