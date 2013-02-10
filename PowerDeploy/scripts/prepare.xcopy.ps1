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
    # nothing to do for xcopy
}

function Reassemble()
{
    $context = Get-PowerDeployContext
    Set-Alias sz "$($context.paths.tools)\7Zip\7za.exe"
    
    sz a -tzip (Join-Path $workDir package.zip) (Join-Path $workDir "out/*") | Out-Null
    
    Remove-Item (Join-Path $workDir "out") -Recurse
}

if ($Disassemble -eq $false -and $Reassemble -eq $false) { "maybe u should have a look at the source code :)" }

if ($Disassemble) { Disassemble }
if ($Reassemble) { Reassemble }

# Generate-Assembly-Info -> google