properties {
    $workDir = $null;
}

#task default -depends Build, Package

# you can put arguments to task in multiple lines using `
task Disassemble `
  -description "TODO" `
{   
    #$context = Get-PowerDeployContext
    #set-alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    #sz "x $packageZip -y o$unzipDir"
    
    Write-Host "work-dir: $workDir"
    Write-Host "nothing todo in disassemble for xcopy"
}

task Reassemble `
  -description "TODO" `
{
    $context = Get-PowerDeployContext
    Set-Alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    sz a -tzip (Join-Path $workDir package.zip) (Join-Path $workDir "out/*")
    
    Remove-Item (Join-Path $workDir "out") -Recurse
}

task Test
{
    Generate-Assembly-Info `
        -file "bla\MefContrib\Properties\SharedAssemblyInfo.cs" `
        -title "MefContrib $version" `
        -description "Community-developed library of extensions to the Managed Extensibility Framework (MEF)." `
        -company "MefContrib" `
        -product "MefContrib $version" `
        -version $version `
        -copyright "Copyright © MefContrib 2009" `
        -clsCompliant "false"
}