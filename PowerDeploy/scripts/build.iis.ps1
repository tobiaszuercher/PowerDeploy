[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$projectFile,
    
    [Parameter(Mandatory = $true, Position = 2)]
    [string]$packageId,
    
    [Parameter(Mandatory = $true, Position = 3)]
    [string]$configPrefix,
    
    [switch]$Build,
    [switch]$Package
)

$context = Get-PowerDeployContext
$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss))

function Build()
{
    Write-Verbose "Building $projectFile"
    
    $packageLocation = Join-Path $workDir "package.zip"
    exec { msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /verbosity:minimal /p:DesktopBuildPackageLocation=$workDir /p:AutoParameterizationWebConfigConnectionStrings=false /p:PackageLocation=$packageLocation /p:IncludeIisSettings=false /p:FilesToIncludeForPublish=OnlyFilesToRunTheApp /p:IncludeSetAclProviderOnDestination=false /t:Package }
}

function Package()
{
    $context = Get-PowerDeployContext

    set-alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    AddPackageParameters 
    
    Remove-Item "$workDir\*.cmd" -Force
    Remove-Item "$workDir\package.deploy-readme.txt"  -Force
    Remove-Item "$workDir\package.SetParameters.xml"  -Force
    Remove-Item "$workDir\package.SourceManifest.xml"  -Force

    sz a -tzip (Join-Path $context.paths.project "deployment/deploymentUnits/$($packageId)_1.0.0.0.zip") "$workDir/*" | Out-Null
    
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
    $xml.WriteAttributeString("id", $packageId)
    $xml.WriteAttributeString("version", "1.3.3.7")
    $xml.WriteAttributeString("environment", "TODO: parseblae env + subenv") # `${env + subenv}
    
    # pass to each individual impl:
    $xml.WriteElementString("appserver", "`${$($configPrefix)_AppServer_Name}")
    $xml.WriteElementString("username", "`${$($configPrefix)_AppServer_Account}")
    $xml.WriteElementString("password", "`${$($configPrefix)_AppServer_Password}")
    $xml.WriteElementString("apppoolname", "`${$($configPrefix)_AppServer_AppPoolName}") # todo: make defaultable
    $xml.WriteElementString("virtualdir", "`${$($configPrefix)_AppServer_Root}")
    $xml.WriteElementString("website", "`${$($configPrefix)_AppServer_WebSite}") # Default Web Site
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}

if ($Build -eq $null -and $Package -eq $null) { Write-Host "wrong usage of this script. maybe you should have a look at the source code :)" }

if ($Build) { Build }
if ($Package) { Package }
