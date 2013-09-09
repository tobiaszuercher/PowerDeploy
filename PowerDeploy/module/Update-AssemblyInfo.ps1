function Update-AssemblyInfo
{
    [CmdletBinding()]
    param(
       [hashtable] $replacements = $null, # example hastable: @{ AssemblyVersion = "1.3.3.7"; AssemblyFileVersion = "1.3.3.7"; AssemblyInformationalVersion = "TOBI IS COOL <3" }
       $path = (Get-Location)
    )

    $pattern = '(?<={0}\(")[^"]*(?="\))'

    Get-ChildItem -Path $path -Recurse -Filter AssemblyInfo.cs | ForEach-Object {
        $filename = $_.FullName

        $file_content = Get-Content $filename | out-string

        $replacements.Keys | % { $file_content = $file_content -replace ($pattern -f $_), $replacements.Item($_).ToString()}
        
        # tfs read-only workaround
        if (Get-ItemProperty "$filename" -name IsReadOnly)
        {
            Set-ItemProperty "$filename" -name IsReadOnly -value $false
            [System.IO.File]::WriteAllText("$filename", $file_content, [Text.Encoding]::UTF8)
            Set-ItemProperty "$filename" -name IsReadOnly -value $true
        }
        else
        {
            [System.IO.File]::WriteAllText("$filename", $file_content, [Text.Encoding]::UTF8)
        }
    }
}