[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$projectFile,
    
    [Parameter(Mandatory = $true, Position = 2)]
    [string]$packageId,
    
    [Parameter(Mandatory = $true, Position = 3)]
    [string]$configPrefix,

    [Parameter(Mandatory = $true, Position = 4)]
    [string]$version,
    
    [switch]$Build,
    [switch]$Package
)

$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd_HH.mm.ss))

function DoBuild()
{
    Write-Host "Building $projectFile"
    $color_before = $Host.UI.RawUI.ForegroundColor
    $Host.UI.RawUI.ForegroundColor = "DarkGray"
    exec { msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /p:OutputPath=$workDir/out /verbosity:minimal /t:Rebuild }
    $Host.UI.RawUI.ForegroundColor = $color_before
}

function Package()
{
    AddPackageParameters 

    # todo: this script shouldn't know anything about those paths...
    sz a -tzip (Join-Path $powerdeploy.paths.project "deployment/deploymentUnits/$($packageId)_$version.zip") "$workDir/*" | Out-Null
    
    #Remove-Item $outputDir -Recurse -Force
}

function AddPackageParameters()
{    
    $file = Join-Path $workDir "package.template.xml"
        
    $xml = New-Object System.Xml.XmlTextWriter($file, $null)
    $xml.Formatting = "Indented"    
    $xml.Indentation = 4
    
    $xml.WriteStartDocument()
    $xml.WriteStartElement("package")
    $xml.WriteAttributeString("type", "xcopy")
    $xml.WriteAttributeString("id", $packageId)
    $xml.WriteAttributeString("version", $version)
    $xml.WriteAttributeString("environment", "`${env}`${subenv}")
    
    # pass to each individual impl:
    $xml.WriteElementString("droplocation", "`${$($configPrefix)_DropLocation}")
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}

if ($Build -eq $null -and $Package -eq $null) { Write-Host "wrong usage of this script. maybe you should have a look at the source code :)" }

if ($Build)
{
    DoBuild
}

if ($Package)
{
    Package
}
