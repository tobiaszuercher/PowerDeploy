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
    Write-Host "Deploying IIS package to web site $package_website_name targeting virtual directory $package_website_path"

    $deploy_svc_url = "$($package_deploy_svc)/deployments?AppPoolName=$($package_pool_name)&AppPoolUser=$($package_pool_user)&AppPoolPassword=$($package_pool_pass)&WebsiteName=$($package_website_name)&AppRoot=$($package_app_root)&PackageId=$($package_name)&PackageVersion=$($package_version)&WebsitePhysicalPath=$($package_website_path)&RuntimeVersion=Version40&WebsitePort=$($package_website_port)"

    Write-Verbose "upload to: $deploy_svc_url"

    try
    {
        $wc = new-object System.Net.WebClient
        $wc.Headers.Add("X-ApiKey", "gugus")
        $wc.UploadFile($deploy_svc_url, ".\package.zip") | Out-Null
        Write-Host "Success"
    }
    catch
    {
        Write-Host "Error deploying. Please check the log files."
        Write-Error $_.Exception.Message
        break
    } 
}

function ShowHelp()
{
    Write-Host " Use package -command where command is one of the following:"
    Write-Host "  -Deploy    Deploys $package_name to $package_website_name ($package_website_path)" # todo: fix those variables
    Write-Host "  -Backup    Backups the currently deployed $package_name"
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

$package_name           = xmlPeek "$package_xml" "/package/@id"
$package_version        = xmlPeek "$package_xml" "/package/@version"
$package_env            = xmlPeek "$package_xml" "/package/@environment"
$package_pool_user      = xmlPeek "$package_xml" "/package/AppPoolUser"
$package_pool_pass      = xmlPeek "$package_xml" "/package/AppPoolPass"
$package_pool_name      = xmlPeek "$package_xml" "/package/AppPoolName"
$package_website_name   = xmlPeek "$package_xml" "/package/WebsiteName"
$package_website_path   = xmlPeek "$package_xml" "/package/WebsitePhysicalPath"
$package_website_port   = xmlPeek "$package_xml" "/package/WebsitePort"
$package_app_root       = xmlPeek "$package_xml" "/package/AppRoot"
$package_app_path       = xmlPeek "$package_xml" "/package/AppPhysicalPath"
$package_deploy_svc     = xmlPeek "$package_xml" "/package/DeployService"

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