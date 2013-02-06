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
$SCRIPT:toolsDir = Join-Path $SCRIPT:rootDir "tools"

$script:pdeploy = @{}
$pdeploy.version = "0.1.0" # contains the current version of PowerDeploy

# todo: $path can be also ./ or relative -> handle this correctly! (not just strings)
function Initialize-PowerDeploy([string]$path = (Split-Path -parent $MyInvocation.MyCommand.path), [string]$projectId)
{
    $SCRIPT:projectId = $projectId
    $SCRIPT:projectRootDir = $path
    $packages = ([xml](Get-Content (Join-Path $projectRootDir "configuration\neutralpackages.xml"))).neutralpackages
    
    $pdeploy.context = new-object system.collections.stack # holds onto the current state of all variables
    $pdeploy.context.push(@{
        "projectId" = $projectId;
        "packages" = $packages.package;
        "paths" = @{
                tools = Join-Path $rootDir "tools";
                project = $path;
                deploymentUnits = Join-Path $projectRootDir "deployment/deploymentUnits";
                deploymentUnitConfigs = Join-Path $projectRootDir "deployment/deploymentUnitConfigs";
                projectConfigFile = Join-Path $projectRootDir "configuration/project/common.xml";
                tasks = Join-Path $rootDir "tasks";
            };
        "environment" = "neutral";
        "deploymentUnits" = @{};
    })
    
    Parse-DeploymentUnits
    
    Set-Alias sz "$($pdeploy.context.peek().paths.tools)\7Zip\7za.exe"
}

function Get-PowerDeployContext()
{
    return $pdeploy.context.peek()
}

function Configure-Environment([string]$environment = "NEUTRAL", [string]$targetPath = $SCRIPT:projectRootDir, [boolean]$deleteTemplates = $false)
{
    Write-Host "Configuring environment for" $environment.ToUpper() -ForegroundColor "DarkCyan"
   
    foreach($template in Get-ChildItem -Path $targetPath -Filter "*.template.*" -Recurse)
    {
        Transform-Template $template.Fullname $environment
        
        if ($deleteTemplates -eq $true)
        {
            Remove-Item $template.Fullname -Force
        }
    }
}

function Prepare-DeploymentUnit([string]$deploymentUnit, [string]$environment) # todo: error handling if one of them is null/empty/not-existing/whatever...
{
    $context = $pdeploy.context.peek()
    
    foreach ($unit in $context.deploymentUnits[$deploymentUnit])
    {
        $foundUnit = Get-ChildItem $context.paths.deploymentUnits -Filter $unit.path
        
        # todo: first check if all ok and then go further with processing
        # otherwise there could be some half transformed deploymentUnits    
        if ($foundUnit -eq $null)
        {
            Write-Host "No neutral package found for $($unit.path)! Please build the neutral package first." -ForegroundColor "Red"
        }
        else
        {
            $workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss))
    
            Set-Alias sz "$($pdeploy.context.peek().paths.tools)\7Zip\7za.exe"        
            sz x -y "-o$($workDir)" $foundUnit.Fullname
            
            $packageType = xmlPeek (Join-Path $workDir "package.template.xml") "package/@type"
            $packageid = xmlPeek (Join-Path $workDir "package.template.xml") "package/@id"
        
            Invoke-psake .\prepare.$packageType.ps1 Disassemble -properties @{ workDir = $workDir }
            
            Configure-Environment $environment $workDir
            
            Invoke-psake .\prepare.$packageType.ps1 Reassemble -properties @{ workDir = $workDir }
            
            $destinationFolder = Join-Path(Join-Path $context.paths.deploymentUnits $deploymentUnit) $environment.ToUpper()
            
            if ([IO.Directory]::Exists($destinationFolder)) { Remove-Item $destinationFolder -Recurse -Force }
            
            New-Item -path $destinationFolder -type directory
            
            
        }
    }
}

# todo: case insensitive! (-> test it, maybe it works already)
# todo: @preparing: tmeplate should be deleted!
function Transform-Template([string]$templateFile, [string]$environment)
{
    $targetFile = $templateFile -replace ".template.", "."
    
    # we don't need to remove read only flag, Force flag of Set-Content does the job
    #Set-ItemProperty $targetFile -name IsReadOnly -value $false
    
    $content = Get-Content $templateFile
    
    $replacePattern = ("{0}\implementation\source\" -f $projectRootDir) -replace "\\", "\\"
    
    Write-Host "transforming $($templateFile -replace $replacePattern) to $($targetFile -replace $replacePattern)"
    
    if ($environment -eq "NEUTRAL")
    {
        $content | Set-Content $targetFile -Force
    }
    else
    {
        Replace-Placeholders $content $environment | Set-Content $targetFile -Force
    }
}  

function Replace-Placeholders([string]$templateText, [string]$environment)
{
    $properties = Get-Properties("local")
    
    $missingProperties = @()

    $MatchEvaluator = 
    {  
      param($match)
    
      $templateParaName = $match -replace "{", "" -replace "}", "" -replace "\$" # todo use $Match 
      
      if (@($properties | where { $_.name -eq $templateParaName}).Count -eq 0)
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
    Write-Output $pdeploy.context.peek().packages
}

function Parse-DeploymentUnits()
{
    $context = $pdeploy.context.peek()

    $result = @{}
    
    # iterate over all deploymentUnit configs and parse them
    foreach($file in Get-ChildItem $context.paths.deploymentUnitConfigs)
    {
        $units = ([xml](get-content $file.Fullname)).deploymentunits.deploymentUnit
    
        $result[$file.BaseName] = $units
    }
    
    $context.deploymentUnits = $result;
}

function Invoke-Build([string]$type)
{
    Invoke-psake (Join-Path (Get-PowerDeployContext).paths.tasks "build.ps1") "Build" -properties @{ type = $type }
    <#$context = $pdeploy.context.peek();
    # todo: handle all!    
    
    # todo: handle if there are multiple projects for $type
    $projectFile = join-path (join-path $projectRootDir "/implementation/source/") ($context.packages | where { $_.type -eq $type }).source
    
    Invoke-psake .\build.$type.ps1 package -properties `
        @{ 
            projectFile = $projectFile; 
            outputDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss)); # i don't understand how i can make in psake task1 a variable which will be available within task2. thats why i pass this over properties (i know, it's nasty)
            toolsDir = $SCRIPT:toolsDir;
            packageId = ($context.packages | where { $_.type -eq $type }).id;
         }
    #>
}

function xmlPeek($filePath, $xpath)
{ 
    [xml] $fileXml = Get-Content $filePath 
    return $fileXml.SelectSingleNode($xpath).Value 
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

Export-ModuleMember -Function Initialize-PowerDeploy, Configure-Environment, List-Packages, Get-Properties, Replace-Text, Invoke-Build, Get-PowerDeployContext, Prepare-DeploymentUnit, xmlpeek, xmlpoke