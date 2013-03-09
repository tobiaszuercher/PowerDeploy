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

$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss))

function DoBuild()
{
    Write-Verbose "Building $project_file"
    
    $packageLocation = Join-Path $workDir "package.zip"
    exec { msbuild $project_file /p:Configuration=Release /p:RunCodeAnalysis=false /verbosity:minimal /p:DesktopBuildPackageLocation=$workDir /p:AutoParameterizationWebConfigConnectionStrings=false /p:PackageLocation=$packageLocation /p:IncludeIisSettings=false /p:FilesToIncludeForPublish=OnlyFilesToRunTheApp /p:IncludeSetAclProviderOnDestination=false /p:DeployIisAppPath="`${$($config_prefix)_AppServer_Website=Default Web Site}`${$($config_prefix)_AppServer_Root=/}$package_id" /t:Package }
}

function Package()
{
    AddPackageParameters 
    
    Remove-Item "$workDir\*.cmd" -Force
    Remove-Item "$workDir\package.deploy-readme.txt" -Force
    Remove-Item "$workDir\package.SetParameters.xml" -Force
    Remove-Item "$workDir\package.SourceManifest.xml" -Force

    sz a -tzip (Join-Path $powerdeploy.paths.project "deployment/deploymentUnits/$($package_id)_$version.zip") "$workDir/*" | Out-Null
    
    # Remove-Item $outputDir -Recurse
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
    $xml.WriteElementString("appserver", "`${$($config_prefix)_AppServer_Name}")
    $xml.WriteElementString("username", "`${$($config_prefix)_AppServer_Account}")
    $xml.WriteElementString("password", "`${$($config_prefix)_AppServer_Password}")
    $xml.WriteElementString("apppoolname", "`${$($config_prefix)_AppServer_AppPoolName=$config_prefix.ToUpper() (`$[env]`$[subenv])}") # todo: make defaultable
    $xml.WriteElementString("virtualdir", "`${$($config_prefix)_AppServer_Root=/}$package_id")
    $xml.WriteElementString("website", "`${$($config_prefix)_AppServer_WebSite}") # Default Web Site
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}

if ($Build -eq $null -and $Package -eq $null) { Write-Host "wrong usage of this script. maybe you should have a look at the source code :)" }

if ($Build) { DoBuild }
if ($Package) { Package }
