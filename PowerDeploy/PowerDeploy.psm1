######################################################################
##
## PowerDeploy
##
## by tobias zürcher
##
######################################################################

$SCRIPT:projectRootDir = $null
$SCRIPT:packages = $null
$SCRIPT:projectId = "moviedemo"
$SCRIPT:rootDir = (Split-Path -parent $MyInvocation.MyCommand.path)
$SCRIPT:lastUsedEnvironment = $null

# todo: $path can be also ./ or relative -> handle this correctly! (not just strings)
function Initialize-PowerDeploy([string]$path = (Split-Path -parent $MyInvocation.MyCommand.path), [string]$projectId)
{
    $SCRIPT:projectId = $projectId
    $SCRIPT:projectRootDir = $path
    $SCRIPT:packages = ([xml](Get-Content (Join-Path $SCRIPT:projectRootDir "configuration\neutralpackages.xml"))).neutralpackages
}

function Configure-Environment([string]$environment = "NEUTRAL")
{
    Write-Host "Configuring environment for" $environment.ToUpper() -ForegroundColor "DarkCyan"
    Write-Host $SCRIPT:rootDir
   
    foreach($template in Get-ChildItem -Path $SCRIPT:projectRootDir -Filter "*.template.*" -Recurse)
    {
        Transform-Template $template.Fullname $environment
    }
}

# todo: case insensitive!
function Transform-Template([string]$templateFile, [string]$environment)
{
    $targetFile = $templateFile -replace ".template.", "."
    
    # we don't need to remove read only flag, Force flag of Set-Content does the job
    #Set-ItemProperty $targetFile -name IsReadOnly -value $false
    
    $content = Get-Content $templateFile
    
    $replacePattern = ("{0}\implementation\source\" -f $projectRootDir) -replace "\\", "\\"
    
    Write-Host "transforming $($templateFile -replace $replacePattern) to $($targetFile -replace $replacePattern)"
    
    Replace-Placeholders $content $environment | Set-Content $targetFile -Force
}

function Replace-Placeholders([string]$templateText, [string]$environment)
{
    $properties = Get-Properties("local")
    
    $missingProperties = @()

    $MatchEvaluator = 
    {  
      param($match)
    
      $templateParaName = $match -replace "{", "" -replace "}", "" -replace "\$" # todo use $Match 
      
      if (@($props | where { $_.name -eq $templateParaName}).Count -eq 0)
      {
        $missingProperties += $templateParaName
      }
      
      return $match.Result(($properties | where { $_.name -eq $templateParaName }).wert)
    }
    
    $result = [regex]::replace($templateText, "\$\{[^\}]*\}", $MatchEvaluator)
    
    if ($missingProperties.Count -gt 0)
    {
        Write-Host "  Missing $($missingProperties.Count) properties" -ForegroundColor "Red"
        $missingProperties | ForEach-Object { write-host "    -> $_" }
    }
    
    return $result
}

# TODO: common xml!
function Get-Properties($environment)
{
    $lastUsedEnvironment = $environment
    return ([xml](Get-Content "C:\git\PowerDeploy\PowerDeploy\environments\$projectId\$environment.xml")).environment.property
}

function List-Packages()
{
    Write-Output $SCRIPT:packages.package
}

function Invoke-Build([string]$type)
{

}

Export-ModuleMember -Function Initialize-PowerDeploy, Configure-Environment, List-Packages, Get-Properties, Replace-Text