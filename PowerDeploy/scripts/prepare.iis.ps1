[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$workDir,
    
    [switch]$Disassemble,
    [switch]$Reassemble
)

# initialize script variables
$SCRIPT:context = Get-PowerDeployContext

function Disassemble()
{   
    Write-Host "[prepare.iis.disassemble]"
    $context = Get-PowerDeployContext
    Set-Alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    $out = Join-Path $workDir "out/"
    
    sz x -y "-o$($out)" (Join-Path $workDir package.zip) | Out-Null
}

function Reassemble()
{
    Write-Host "[prepare.iis.reassemble]"
    $context = Get-PowerDeployContext
    Set-Alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    sz a -tzip (Join-Path $workDir package.zip) (Join-Path $workDir "out/*") | Out-Null
    
    Remove-Item (Join-Path $workDir "out") -Recurse
}

if ($Disassemble -eq $false -and $Reassemble -eq $false) { "maybe u should have a look at the source code :)" }

if ($Disassemble) { Disassemble }
if ($Reassemble) { Reassemble }

# Generate-Assembly-Info -> google