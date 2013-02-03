properties {
  $projectFile = $null,
  $configId = $null,
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
    exec { msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /p:OutputPath=$outputDir /t:Rebuild }
}

task Package `
  -description "Packages the build output into one single zip" `
  -depends Build `
{
    $context = Get-PowerDeployContext
    set-alias sz C:\git\PowerDeploy\PowerDeploy\tools\7Zip\7za.exe

    sz a -tzip (Join-Path $context.paths.project "deployment/deploymentUnits/$($configId)_1.0.0.0.zip") "$outputDir/*"
}