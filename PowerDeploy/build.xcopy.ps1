properties {
  $projectFile = $null,
  $packageId = $null,
  $outputPath = $null,
  $outputDir = $null,
  $toolsDir = $null
}

task default -depends Build, Package

# you can put arguments to task in multiple lines using `
task Build `
  -description "This task shows how to make a variable required to run task. Run this script with -properties @{projectFile = test.csproj}" `
  -requiredVariables projectFile `
{   
    write-host "output: $outputDir"
    exec { msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /p:OutputPath=$outputDir/out /verbosity:minimal /t:Rebuild }
}

task Package `
  -description "Packages the build output into one single zip" `
  -depends Build, AddPackageParameters `
{
    $context = Get-PowerDeployContext
    set-alias sz C:\git\PowerDeploy\PowerDeploy\tools\7Zip\7za.exe

    sz a -tzip (Join-Path $context.paths.project "deployment/deploymentUnits/$($packageId)_1.0.0.0.zip") "$outputDir/*"
    
    # Remove-Item $outputDir -Recurse
}

task AddPackageParameters `
{    
    $file = Join-Path $outputdir "package.template.xml"
    
    $xml = New-Object System.Xml.XmlTextWriter($file, $null)
    $xml.Formatting = "Indented"
    $xml.Indentation = 4
    
    $xml.WriteStartDocument()
    $xml.WriteStartElement("package")
    $xml.WriteAttributeString("type", "xcopy")
    $xml.WriteAttributeString("id", "Bla")
    $xml.WriteAttributeString("version", "1.3.3.7")
    $xml.WriteAttributeString("environment", "") # `${env + subenv}
    
    # pass to each individual impl:
    $xml.WriteElementString("dbserver", "`${MOVIE_DB_Server}")
    $xml.WriteElementString("dbname", "`${MOVIE_DB_Name}")
    $xml.WriteEndElement()
    $xml.WriteEndDocument()
    
    $xml.Flush()
    $xml.Close()
}