function xmlPeek($filePath, $xpath)
{ 
    [xml] $fileXml = Get-Content $filePath 
    $found = $fileXml.SelectSingleNode($xpath)

    if ($found -eq $null) { return "" }

    if ($found.GetType().Name -eq 'XmlAttribute') { return $found.Value }

    return $found.InnerText
} 

function xmlPoke($file, $xpath, $value)
{ 
    $filePath = $file.FullName 

    [xml] $fileXml = Get-Content $filePath 
    $node = $fileXml.SelectSingleNode($xpath) 
    
    if ($node)
    { 
        $node.Value = $value 

        $fileXml.Save($filePath)  
    } 
}

function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ("Error in {0}" -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

function Task
{
    [CmdletBinding()]  
    param(
        [Parameter(Position=0,Mandatory=1)][string]$name = $null,
        [Parameter(Position=1,Mandatory=0)][scriptblock]$action = $null,
        [Parameter(Position=9,Mandatory=0)][string]$description = $null,
        [Parameter(Position=10,Mandatory=0)][string]$alias = $null
    )
}

function CheckWheterEnvironmentExist($environment)
{
    return (Test-Path (Join-Path $powerdeploy.paths.environments "$($powerdeploy.project.id)/$environment.xml")) -or $environment.ToUpper() -eq "NEUTRAL"
}

# creates an unique folder. i like to have the date in the folder name for traceability while debugging...
# this can be refactored to just a guid as soon things gets more stable 
function createUniqueDir()
{
    return '{0}___{1}' -f (Get-Date -Format yyyy-MM-dd_HH.mm.ss), [guid]::NewGuid().ToString().Substring(6)
}
  
function Transform-Template([string]$template_file, [string]$environment, [string]$subenv = '')
{
    $target_file = $template_file -replace ".template.", "."
    
    # we don't need to remove read only flag, Force flag of Set-Content does the job
    #Set-ItemProperty $target_file -name IsReadOnly -value $false
    
    $content = Get-Content $template_file | Out-String
    
    $replace_pattern = ("{0}\implementation\source\" -f $powerdeploy.paths.project) -replace "\\", "\\"
    
    Write-Verbose "transforming $($template_file -replace $replace_pattern) to $($target_file -replace $replace_pattern)"
    
    if ($environment -eq "NEUTRAL")
    {
        $content | Set-Content $target_file -Force
    }
    else
    {
        Replace-Placeholders $content $environment $subenv| Set-Content $target_file -Force
    }
}  

function Replace-Placeholders($template_text, [string]$environment, [string]$subenv = '')
{
    $properties = Get-Properties($environment)

    $MatchEvaluator = 
    {  
        param($match)

        $property_name = $match.Groups["Name"].Value

        $value_available = @($properties | where { $_.name -eq $property_name}).Count -eq 1

        if ($property_name -eq 'env')
        {
            Write-Verbose "$property_name -> $environment"
            return $environment
        }

        if ($property_name -eq 'subenv')
        {
            Write-Verbose "$property_name -> $subenv"
            return $subenv
        }

        if ($property_name.Contains('=') -and $value_available -eq $false)
        {
            # it's a default proppy and no value found in environment -> use default value
            $found_value = $property_name.Split('=')[1] -replace "\$\[env\]","$environment" -replace "\$\[subenv\]",$subenv

            Write-Verbose "$property_name -> $found_value (used default value)"
            
            return $found_value
        }

        if ($value_available)
        {
            $found_value = (($properties | where { $_.name -eq $property_name }).Value) -replace "\$\{env\}","$environment" -replace "\$\{subenv\}","$subenv"
            
            Write-Verbose "$property_name -> $found_value"
            
            return $found_value
        }

        Write-Host "  -> missing environment property $property_name" -ForegroundColor Red

        return "MISSING_PROPERTY: $property_name"
    }
    
    $result = [regex]::replace($template_text, "\$\{(?<Name>[^\}]+)\}", $MatchEvaluator)
        
    return $result
}

# TODO: merge with common xml!
function Get-Properties($environment)
{
    return ([xml](Get-Content (Join-Path $powerdeploy.paths.environments "$($powerdeploy.project.id)\$environment.xml"))).environment.property
}

function Test-Administrator  
{  
    $user = [Security.Principal.WindowsIdentity]::GetCurrent();
    (New-Object Security.Principal.WindowsPrincipal $user).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)  
}