[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$projectFile,
    
    [Parameter(Mandatory = $true, Position = 2)]
    [string]$packageId,
    
    [switch]$Build,
    [switch]$Package
)

$context = Get-PowerDeployContext
$workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss))

function Build()
{
    Write-Verbose "Building $projectFile"
    #todo: exec
    exec { 
    msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /p:OutputPath=$workDir/out /verbosity:minimal /t:Rebuild 
    }
}

function Package()
{
    Write-Verbose "Packaging..."
    Write-Verbose "PackageId: $packageId"
    Write-Verbose "WorkDir: $workDir"
    
    $context = Get-PowerDeployContext

    set-alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    AddPackageParameters 

    sz a -tzip (Join-Path $context.paths.project "deployment/deploymentUnits/$($packageId)_1.0.0.0.zip") "$workDir/*"
    
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
    $xml.WriteAttributeString("type", "xcopy")
    $xml.WriteAttributeString("id", "Bla")
    $xml.WriteAttributeString("version", "1.3.3.7")
    $xml.WriteAttributeString("environment", "TODO: parseblae env + subenv") # `${env + subenv}
    
    # pass to each individual impl:
    $xml.WriteElementString("dbserver", "`${MOVIE_DB_Server}")
    $xml.WriteElementString("dbname", "`${MOVIE_DB_Name}")
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}

if ($Build -eq $null -and $Package -eq $null) { Write-Host "wrong usage of this script. maybe you should have a look at the source code :)" }

if ($Build) { Build }
if ($Package) { Package }
