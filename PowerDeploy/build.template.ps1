properties {
  $projectFile = $null,
  $configId = $null,
  $outputPath = $null,
  $outputDir = $null,
  $toolsDir = $null
}

task default -depends Build, Package

task Build `
  -description "This task shows how to make a variable required to run task. Run this script with -properties @{projectFile = test.csproj}" `
  -requiredVariables projectFile, outputDir `
{   
}

task Package `
  -description "Packages the build output into one single zip" `
  -depends Build `
{
}