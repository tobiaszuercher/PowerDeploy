[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$workDir,
    
    [switch]$Disassemble,
    [switch]$Reassemble
)

$startup_location = Get-Location

$out_dir = Join-Path "$workDir" "out"
$unzip_dir = Join-Path "$workDir" "unzipped"
$package_zip = Join-Path "$workDir" "package.zip"

Write-Verbose "workDir: $workDir"
Write-Verbose "unzip_dir: $unzip_dir"
Write-Verbose "package_zip: $package_zip"

function Disassemble()
{   
    # nothing to do for xcopy, everything is in $out_dir
}

function Reassemble()
{
    Write-Verbose "Reassembling..."

    #The App.config has wrong name, it needs *ProjectName*.exe.config -> patch it!
    $config_file_name = (Get-ChildItem "$out_dir" -Filter *.exe.config).Name
    $config_file_template = Join-Path "$out_dir" "App.config"
    
    if ($config_file_name -ne $null -and (Test-Path $config_file_template) -eq $true)
    {
        Write-Verbose "Found an App.config with exe.config -> replace it"
        $config_file_path = Join-Path "$out_dir" $config_file_name

        Remove-Item $config_file_path -Force
        Move-Item $config_file_template ($config_file_path) -Force
    }
    
    if (Test-Path $package_zip)
    {
        Remove-Item $package_zip -Force
    }

    Write-Verbose "Zipping output to package.zip"
    sz a -tzip $package_zip "$out_dir/*" | Out-Null
    
    Remove-Item $out_dir -Recurse
}

if ($Disassemble -eq $false -and $Reassemble -eq $false) { "maybe u should have a look at the source code :)" }

if ($Disassemble) { Disassemble }
if ($Reassemble) { Reassemble }

# Generate-Assembly-Info -> google