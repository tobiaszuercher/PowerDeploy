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

$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) ([guid]::NewGuid().ToString()))

function DoBuild()
{
    Write-Host "Building $projectFile"
    Write-Verbose "Working dir: $workDir"
    $color_before = $Host.UI.RawUI.ForegroundColor
    $Host.UI.RawUI.ForegroundColor = "DarkGray"

    # todo rename pre-/post deployment scripts
    Push-Location (Split-Path -parent $projectfile)
    exec { msbuild $projectFile /p:Configuration=Release /p:OutputPath=$workDir/build /p:VisualStudioVersion=11.0 /verbosity:minimal /t:Rebuild }
    Pop-Location
    $Host.UI.RawUI.ForegroundColor = $color_before
}

function Package()
{
    New-Item -path "$workDir/package/scripts" -type directory | Out-Null

    AddPackageParameters 

    # copy script folders
    if (Test-Path "$workDir/build/Objects")     { Copy-Item "$workDir/build/Objects" "$workDir/package/scripts/" -Recurse }
    if (Test-Path "$workDir/build/DataSets")    { Copy-Item "$workDir/build/DataSets" "$workDir/package/scripts/" -Recurse }
    if (Test-Path "$workDir/build/Migrations")  { Copy-Item "$workDir/build/Migrations" "$workDir/package/scripts/Migrations" -Recurse }
    if (Test-Path "$workDir/build/Before")      { Copy-Item "$workDir/build/Before" "$workDir/package/scripts/Before" -Recurse }
    if (Test-Path "$workDir/build/After")       { Copy-Item "$workDir/build/After" "$workDir/package/scripts/After" -Recurse }

    # rename all sql files to .template.sql (so ${} vars can be used)
    Get-ChildItem "$workDir/package/" -Recurse -Filter *.sql | % { rename-Item "$($_.Fullname)" "$($_.Basename).template.sql" }

    sz a -tzip (Join-Path $powerdeploy.paths.project "deployment/deploymentUnits/$($packageId)_$version.zip") "$workDir/package/*" | Out-Null
    
    Remove-Item $workDir -Recurse -Force
}

function AddPackageParameters()
{    
    $file = Join-Path $workDir "package/package.template.xml"
        
    $xml = New-Object System.Xml.XmlTextWriter($file, $null)
    $xml.Formatting = "Indented"    
    $xml.Indentation = 4
    
    $xml.WriteStartDocument()
    $xml.WriteStartElement("package")
    $xml.WriteAttributeString("type", "db")
    $xml.WriteAttributeString("id", $packageId)
    $xml.WriteAttributeString("version", $version)
    $xml.WriteAttributeString("environment", "`${env}`${subenv}")
    
    # pass to each individual impl:
    $xml.WriteElementString("dbserver", "`${$($configPrefix)_Database_Server}")
    $xml.WriteElementString("dbname", "`${$($configPrefix)_Database_Name}")
    $xml.WriteElementString("datadeploy", "`${$($configPrefix)_Database_DataDeploy=Empty}")

    # Empty -> no data deployments (default)
    # dataset:name -> run the scripts form the dataset folder
    # env:name -> use database from another environment
    # unc:\\share\backup.bak -> import from unc path. it's up to you to schedule any backup.

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
