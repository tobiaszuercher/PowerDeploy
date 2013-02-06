properties {
	$context = Get-PowerDeployContext
}

task default -depends Build

task Build `
    -requiredVariables context
{   
    # todo: handle all!    
    
    # todo: handle if there are multiple projects for $type
    $projectFile = Join-Path (Join-Path $context.paths.project "/implementation/source/") ($context.packages | where { $_.type -eq $type }).source
    
    Invoke-psake .\build.$type.ps1 package -properties `
        @{ 
            projectFile = $projectFile; 
            outputDir = (Join-Path (Join-Path $env:TEMP PowerDeploy) (Get-Date -Format yyyy-MM-dd__HH.mm.ss)); # i don't understand how i can make in psake task1 a variable which will be available within task2. thats why i pass this over properties (i know, it's nasty)
            toolsDir = $SCRI;
            packageId = ($context.packages | where { $_.type -eq $type }).id;
         }
}