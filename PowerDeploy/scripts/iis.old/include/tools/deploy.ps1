[CmdletBinding()]
Param(
    [Parameter()] [string] $backup_package = '',
    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help,
    [switch] $Rollback,
    [switch] $UpdateExisting
)

$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

Push-Location "$work_dir"

Start-Transcript "deploy.txt"

function Backup()
{
	Write-Host "Create backup for $package_appserver/$package_virtualdir"

	$color = $Host.UI.RawUI.ForegroundColor
	$Host.UI.RawUI.ForegroundColor = "DarkGray"

	# because of the bug mentioned in the link below, we have to make a workaround
	# http://connect.microsoft.com/PowerShell/feedback/details/376207/executing-commands-which-require-quotes-and-variables-is-practically-impossible

	# .\tools\MsDeploy\x64\msdeploy.exe -source:"contentPath='$package_website/$package_virtualdir'" -dest:"package='backup_$(Get-Date -Format yyyy-MM-dd_HH-mm-ss).zip'" -verb:sync
	cmd.exe /C $("tools\MsDeploy\x64\msdeploy.exe -verb:sync -source:contentPath=`"{0}`" -dest:package=`"{1}`"" -f "$package_website/$package_virtualdir", "backup_$(Get-Date -Format yyyy-MM-dd_HH-mm-ss).zip")
	
	$Host.UI.RawUI.ForegroundColor = $color

	Write-Host
}

function Rollback()
{
	cls

	if ($backup_package -eq '')
	{
		Write-Host "Following backups available:"
		Write-Host 

		Get-ChildItem -filter backup_* | Format-table Name -HideTableHeaders

		Write-Host "Usage example: " -nonewline
		Write-Host "  package -Rollback backup_2000-01-12_13:37.zip" -f Red
		Write-Host
	}
	else
	{
		Write-Host "Restoring package $backup_package"
		Write-Host 

		# remove optional .\ in front of filename (just if the juster pressed Format-table)
		DoDeploy (Get-Item "$backup_package").Fullname
	}
}

function DoDeploy([string] $package = "package.zip")
{
	Write-Host "Deploying IIS package to web site $package_website into virtual directory $package_virtualdir"

	Import-Module .\tools\PowerDeploy.Extensions.dll -DisableNameChecking

	# todo get-architecture
	$Host.UI.RawUI.ForegroundColor = "DarkGray"
	.\tools\MsDeploy\x64\msdeploy.exe -source:package="$package" -dest:"auto,includeAcls='False',computerName='$package_appserver',authType='NTLM'" -verb:sync -disableLink:ContentExtension -disableLink:CertificateExtension -allowUntrusted
 	$Host.UI.RawUI.ForegroundColor = "Gray"

	Write-Host

	Create-AppPool -Name $package_apppoolname -ServerName $package_appserver -WAMUserName $package_username -WAMUserPass -package_password
	Assign-AppPool -ServerName $package_appserver -ApplicationPoolName $package_apppoolname -VirtualDirectory $package_virtualdir -WebsiteName $package_website

	Remove-Module PowerDeploy.Extensions

	Write-Host "Done! have fun!"
}

function ShowHelp()
{
	Write-Host " Use package -command where command is one of the following:"
	Write-Host "  -Deploy	 Deploys $package_name to $package_appserver$package_virtualdir"
	Write-Host "  -Backup	 Backups the currently deployed $package_name on $package_appserver."
	Write-Host "  -Rollback	 Rollbacks a previously created backup."
	Write-Host "  -Help 	 Shows help information."
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
	cls
	
	Write-Host ""
	Write-Host ""
	Write-Host "Welcome to your power deploy shell!"
	Write-Host ""
	Write-Host "  Package: " -nonewline
	Write-Host $package_name v$package_version -ForegroundColor Red -nonewline
	Write-Host ""
	Write-Host ("           targeting {0}" -f $package_env.ToUpper())
	Write-Host "" 
	Write-Host ""
}


$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

$package_xml         = Join-Path "$work_dir" "package.xml"

$package_name 		 = xmlPeek "$package_xml" "/package/@id"
$package_version     = xmlPeek "$package_xml" "/package/@version"
$package_env         = xmlPeek "$package_xml" "/package/@environment"
$package_appserver   = xmlPeek "$package_xml" "/package/appserver"
$package_username    = xmlPeek "$package_xml" "/package/username"
$package_password    = xmlPeek "$package_xml" "/package/password"
$package_apppoolname = xmlPeek "$package_xml" "/package/apppoolname"
$package_virtualdir  = xmlPeek "$package_xml" "/package/virtualdir"
$package_website     = xmlPeek "$package_xml" "/package/website"

if ($Deploy -eq $false -and $Backup -eq $false -and $Rollback -eq $false)
{
	$Help = $true
}

if ($Help)
{
	Write-Welcome
	ShowHelp
}

if ($Backup) { Backup }
if ($Rollback) { Rollback }
if ($Deploy) 
{
	Backup
	DoDeploy
}

Stop-Transcript

Pop-Location