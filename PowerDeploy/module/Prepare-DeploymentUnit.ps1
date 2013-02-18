function Prepare-DeploymentUnit([string]$deployment_unit, [string]$environment) # todo: error handling if one of them is null/empty/not-existing/whatever...
{
    # make sure we have the actual version
    Import-Configurations
    Import-DeploymentUnits
    
    # remove target folder
    $destination_folder = Join-Path (Join-Path $powerdeploy.paths.deployment_units $deployment_unit) $environment.ToUpper()
    
    if ([IO.Directory]::Exists($destination_folder))
    {
        Write-Verbose "Remove existing deployment units."
        Remove-Item $destination_folder -Recurse -Force
    }
    
    New-Item -path $destination_folder -type directory | Out-Null
    
    foreach ($unit in $powerdeploy.deployment_units[$deployment_unit])
    {
        $found_unit = Get-ChildItem $powerdeploy.paths.deployment_units -Filter $unit.path
      
        # todo: first check if all ok and then go further with processing
        # otherwise there could be some half transformed deploymentUnits    
        if ($found_unit -eq $null)
        {
            Write-Host "No neutral package found for $($unit.path)! Please build the neutral package first." -ForegroundColor "Red"
        }
        else
        {
            $workDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (createUniqueDir))
    
#            Set-Alias sz "$($powerdeploy.paths.tools)\7Zip\7za.exe"        
            sz x -y "-o$($workDir)" $found_unit.Fullname | Out-Null
            
            $packagePath = Join-Path $workDir "package.template.xml"
            $packageType = xmlPeek $packagePath "package/@type"
            $packageId = xmlPeek $packagePath "package/@id"
            $packageVersion = xmlPeek $packagePath "package/@version"
            
            Write-Host "Preparing $packageId with type $packageType" -ForegroundColor yellow
            
            Invoke-Expression -Command "$(Join-Path $($powerdeploy).paths.scripts prepare.$packageType.ps1) $workDir -Disassemble"
            
            Configure-Environment $environment $workDir -deleteTemplates $true
            
            Invoke-Expression -Command "$(Join-Path $($powerdeploy).paths.scripts prepare.$packageType.ps1) $workDir -Reassemble"
            
            Copy-Item "$workDir\" "$destination_folder\$($packageId)_$($packageVersion)\" -Recurse

            # copy deploy scripts & tools
            Copy-Item (Join-Path $powerdeploy.paths.scripts "deploy/$packageType/*") "$destination_folder\$($packageId)_$($packageVersion)\" -Recurse
        }
    }
}