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
    return (Test-Path (Join-Path $powerdeploy.paths.environments "$($powerdeploy.project.id)/$environment.xml"))
}

# creates an unique folder. i like to have the date in the folder name for traceability while debugging...
# this can be refactored to just a guid as soon things gets more stable 
function createUniqueDir()
{
    return '{0}___{1}' -f (Get-Date -Format yyyy-MM-dd_HH.mm.ss), [guid]::NewGuid().ToString().Substring(6)
}

# this from config-env


# check if this is the better format-string...
function Format-String([string]$string, [hashtable]$replacements)
{
    $current_index = 0
    $replacment_list = @()

    foreach($key in $replacements.Keys)
    {
        $inputPattern = '\${' + $key + '}'
        $replacementPattern = '{${1}' + $current_index + '${2}}'
        $string = $string -replace $inputPattern, $replacementPattern
        $replacment_list += $replacements[$key]
        
        write-host $inputPattern $replacementPattern

        $current_index++
    }
    
    return $string -f $replacment_list
}
    

# todo: case insensitive! (-> test it, maybe it works already)
function Transform-Template([string]$template_file, [string]$environment)
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
        Replace-Placeholders $content $environment | Set-Content $target_file -Force
    }
}  

function Replace-Placeholders($template_text, [string]$environment)
{
    $properties = Get-Properties($environment)
    
    $missing_properties = @()

    $MatchEvaluator = 
    {  
      param($match)
    
      $templateParaName = $match -replace "{", "" -replace "}", "" -replace "\$" # todo use $Match 
      
      if (@($properties | where { $_.name -eq $templateParaName}).Count -eq 0)
      {
        $missing_properties += $templateParaName
      }
      
      return $match.Result(($properties | where { $_.name -eq $templateParaName }).value)
    }
    
    $result = [regex]::replace($template_text, "\$\{[^\}]*\}", $MatchEvaluator)
    
    if ($missing_properties.Count -gt 0)
    {
        Write-Host "  Missing $($missing_properties.Count) properties" -ForegroundColor "Red"
        $missing_properties | ForEach-Object { write-host "    -> $_" }
    }
    
    return $result
}

# TODO: merge with common xml!
function Get-Properties($environment)
{
    return ([xml](Get-Content (Join-Path $powerdeploy.paths.environments "$($powerdeploy.project.id)\$environment.xml"))).environment.property
}