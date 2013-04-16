[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$work_dir,
    
    [switch]$Disassemble,
    [switch]$Reassemble
)

function Disassemble()
{
	# nothing to do here, package is already ready
}

function Reassemble()
{
    sz a -tzip (Join-Path "$work_dir" "package.zip") "$work_dir/scripts/*" | Out-Null

    Remove-Item -Force -Recurse "$work_dir/scripts"
}

if ($Disassemble -eq $false -and $Reassemble -eq $false) { "maybe u should have a look at the source code :)" }

if ($Disassemble) { Disassemble }
if ($Reassemble) { Reassemble }