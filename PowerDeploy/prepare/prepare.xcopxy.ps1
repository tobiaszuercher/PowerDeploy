properties {

}

#task default -depends Build, Package

# you can put arguments to task in multiple lines using `
task Disassemble `
  -description "TODO" `
{   
    write-host "output: $outputDir"
    exec { msbuild $projectFile /p:Configuration=Release /p:RunCodeAnalysis=false /p:OutputPath=$outputDir /t:Rebuild }
}

task Reassemble `
  -description "TODO" `
{
    $context = Get-PowerDeployContext
    set-alias sz C:\git\PowerDeploy\PowerDeploy\tools\7Zip\7za.exe

    sz a -tzip (Join-Path $context.paths.project "deployment/deploymentUnits/$($configId)_1.0.0.0.zip") "$outputDir/*"
}