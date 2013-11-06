[CmdletBinding()]
Param(
    [Parameter()] [string] $backup_package = '',
    [switch] $Deploy,
    [switch] $Backup,
    [switch] $Help,
    [switch] $Rollback,
    [switch] $UpdateExisting
)
$global:ErrorActionPreference = "Stop"
$work_dir = resolve-path "$(Split-Path -parent $MyInvocation.MyCommand.path)/.."

Push-Location "$work_dir"

#Start-Transcript "deploy.txt"

function DoDeploy([string] $package = "package.zip")
{
    Write-Host "Deploying IIS package to web site $package_website into virtual directory $package_website_path"

    $share_folder = Join-Path $package_path ([guid]::NewGuid().ToString())

    # todo: add try catch
    # create folder in share location and copy all needed files
    New-Item -Path $share_folder -ItemType Directory

    Copy-Item .\package.zip "$share_folder" -Verbose
    Copy-Item .\package.xml "$share_folder" -Verbose
    
    .\tools\iis.deploy.ps1 -WebsiteName $package_website -WebsitePhysicalPath $package_website_path -AppPoolName $package_apppoolname -AppPoolUser $package_username -AppPoolPass $package_password -AppName $package_name -AppPhysicalPath $package_app_path -WebSiteRoot $package_website_root -PackagePath $share_folder
}

function ShowHelp()
{
    Write-Host " Use package -command where command is one of the following:"
    Write-Host "  -Deploy    Deploys $package_name to $package_appserver$package_virtualdir" # todo: fix those variables
    Write-Host "  -Backup    Backups the currently deployed $package_name on $package_appserver."
    Write-Host "  -Rollback  Rollbacks a previously created backup."
    Write-Host "  -Help      Shows help information."
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

$package_xml         = Join-Path "$work_dir" "package.xml"

$package_name        = xmlPeek "$package_xml" "/package/@id"
$package_version     = xmlPeek "$package_xml" "/package/@version"
$package_env         = xmlPeek "$package_xml" "/package/@environment"
$package_appserver   = xmlPeek "$package_xml" "/package/appserver"
$package_username    = xmlPeek "$package_xml" "/package/username"
$package_app_path    = xmlPeek "$package_xml" "/package/appphysicalpath"
$package_password    = xmlPeek "$package_xml" "/package/password"
$package_apppoolname = xmlPeek "$package_xml" "/package/apppoolname"
$package_website     = xmlPeek "$package_xml" "/package/websitename"
$package_website_path = xmlPeek "$package_xml" "/package/websitepath"
$package_path        = xmlPeek "$package_xml" "/package/packagepath"


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
    DoDeploy
}

#Stop-Transcript

Pop-Location