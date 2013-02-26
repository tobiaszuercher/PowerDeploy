# todo: configure makes two newlines at the end of the file
# todo: it should copy the template first and then replace the values...

function Configure-Environment
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0)] [string]$environment = "NEUTRAL",
        [Parameter(Position = 1)] [string]$target_path = $powerdeploy.paths.project, 
        [Parameter(Position = 2)] [boolean]$delete_templates = $false
    )
    
    process
    {
        #todo: check wheter this environment exists!

        Write-Host "Configuring environment for" $environment.ToUpper() " in $target_path" -ForegroundColor "DarkCyan"
       
        foreach($template in Get-ChildItem -Path $target_path -Filter "*.template.*" -Recurse)
        {
            Transform-Template $template.Fullname $environment
            
            if ($delete_templates -eq $true)
            {
                Remove-Item $template.Fullname -Force
            }
        }
    }
}

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