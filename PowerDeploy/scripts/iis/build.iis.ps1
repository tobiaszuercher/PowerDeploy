[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$project_file,
    
    [Parameter(Mandatory = $true, Position = 2)]
    [string]$package_id,
    
    [Parameter(Mandatory = $true, Position = 3)]
    [string]$config_prefix,

    [Parameter(Mandatory = $true, Position = 4)]
    [string]$version,
    
    [switch]$Build,
    [switch]$Package
)

$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) ([guid]::NewGuid().ToString()))
$packageLocation = Join-Path $workDir "unzipped"

function DoBuild()
{
    Write-Verbose "Building $project_file"
    
    Write-Host $packageLocation
    exec { msbuild $project_file /p:Configuration=Release /p:RunCodeAnalysis=false /verbosity:minimal /p:AutoParameterizationWebConfigConnectionStrings=false /p:IncludeIisSettings=false /p:FilesToIncludeForPublish=OnlyFilesToRunTheApp /p:IncludeSetAclProviderOnDestination=false /p:DeployOnBuild=true /p:DeployTarget=Package /p:_PackageTempDir="$packageLocation" /t:Rebuild /t:Package }
}

function Package()
{
    AddPackageParameters 

    # msbuild extracts iis-content to $packageLocation, zip that into package.zip
    sz a -tzip "$workDir\package.zip" "$packageLocation/*" | Out-Null
    Remove-Item "$packageLocation" -Recurse -Force

    # zip neutral package
    sz a -tzip (Join-Path $powerdeploy.paths.project "deployment/deploymentUnits/$($package_id)_$version.zip") "$workDir/*" | Out-Null
    
    # remove temp folder
    Remove-Item $workDir -Recurse -Force
}

function AddPackageParameters()
{    
    $file = Join-Path $workDir "package.template.xml"
    
    $xml = New-Object System.Xml.XmlTextWriter($file, $null)
    $xml.Formatting = "Indented"
    $xml.Indentation = 4
    
    $xml.WriteStartDocument()
    $xml.WriteStartElement("package")
    $xml.WriteAttributeString("type", "iis")
    $xml.WriteAttributeString("id", $package_id)
    $xml.WriteAttributeString("version", $version)
    $xml.WriteAttributeString("environment", "`${env}`${subenv}")
    
    # pass to each individual impl:
    $xml.WriteElementString("AppPoolUser", "`${$($config_prefix)_AppServer_AppPool_User}")
    $xml.WriteElementString("AppPoolPass", "`${$($config_prefix)_AppServer_AppPool_Pass}")
    $xml.WriteElementString("AppPoolName", "`${$($config_prefix)_AppServer_AppPool_Name=$config_prefix (`$[env]`$[subenv])}") # todo: make defaultable
    $xml.WriteElementString("WebsiteName", "`${$($config_prefix)_AppServer_WebSite_Name}")
    $xml.WriteElementString("WebsitePhysicalPath", "`${$($config_prefix)_AppServer_WebSite_PhysicalPath}")
    $xml.WriteElementString("WebsitePort", "`${$($config_prefix)_AppServer_Website_Port=80}")
    $xml.WriteElementString("AppRoot", "`${$($config_prefix)_AppServer_App_Root=/}")
    $xml.WriteElementString("AppPhysicalPath", "`${$($config_prefix)_App_PhysicalPath=}")
    $xml.WriteElementString("DeployService", "`${$($config_prefix)_DeployService}")
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}

if ($Build -eq $null -and $Package -eq $null) { Write-Host "wrong usage of this script. maybe you should have a look at the source code :)" }

if ($Build) { DoBuild }
if ($Package) { Package }