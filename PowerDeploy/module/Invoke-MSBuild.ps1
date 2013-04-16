<#
.Synopsis
Compiles a project using msbuild.exe.

.Description
The Invoke-MSBuild cmdlet is used to compile a MSBuild-compatible project or solution. You should always use this cmdlet instead of a direct call to msbuild.exe or existing cmdlets you may have found online when working with powerdelivery.

This cmdlet provides the following essential continuous delivery features:

Updates the version of any AssemblyInfo.cs (or AssemblyInfo.vb) files with the current build version. This causes all of your binaries to have the build number. For example, if your build pipeline's version in the script is set to 1.0.2 and this is a build against changeset C234, the version of your assemblies will be set to 1.0.2.234.

Automatically targets a build configuration matching the environment name ("Commit", "Test", or "Production"). Create build configurations named "Commit", "Test", and "Production" with appropriate settings in your projects for this to work. If you don't want this, you'll have to explicitly pass the configuration as a parameter.

Reports the status of the compilation back to TFS to be viewed in the build summary. This is important because it allows tests run using mstest.exe to have their run results associated with the compiled assets created using this cmdlet.

.Example
Invoke-MSBuild MyProject/MySolution.sln -properties  @{MyCustomProp = SomeValue}

.Parameter projectFile
A relative path at or below the script directory that specifies an MSBuild project or solution to compile.

.Parameter properties
Optional. A PowerShell hash containing name/value pairs to set as MSBuild properties.

.Parameter target
Optional. The name of the MSBuild target to invoke in the project file. Defaults to the default target specified within the project file.

.Parameter toolsVersion
Optional. The version of MSBuild to run ("2.0", "3.5", "4.0", etc.). The default is "4.0".

.Parameter verbosity
Optional. The verbosity of this MSBuild compilation. The default is "m".

.Parameter buildConfiguration
Optional. The default is to use the same as the environment name. Create build configurations named "Commit", "Test", and "Production" with appropriate settings in your projects.

.Parameter flavor
Optional. The platform configuration (x86, x64 etc.) of this MSBuild complation. The default is "AnyCPU".

.Parameter ignoreProjectExtensions
Optional. A semicolon-delimited list of project extensions (".smproj;.csproj" etc.) of projects in the solution to not compile.

.Parameter dotNetVersion
Optional. The .NET version to use for compilation. Defaults to the version specified in the project file(s) being built.
#>
function Invoke-MSBuild {
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][string] $projectFile, 
        [Parameter(Position=1,Mandatory=0)] $properties, 
        [Parameter(Position=2,Mandatory=0)][string] $target, 
        [Parameter(Position=3,Mandatory=0)][string] $toolsVersion, 
        [Parameter(Position=4,Mandatory=0)][string] $verbosity = "m", 
        [Parameter(Position=5,Mandatory=0)][string] $buildConfiguration = "Debug", 
        [Parameter(Position=6,Mandatory=0)][string] $flavor = "AnyCPU", 
        [Parameter(Position=7,Mandatory=0)][string] $ignoreProjectExtensions, 
        [Parameter(Position=8,Mandatory=0)][string] $dotNetVersion = "4.0"
    )
	
	$dropLocation = Get-BuildDropLocation
	$logFolder = Join-Path $dropLocation "Logs"
	mkdir -Force $logFolder | Out-Null

    $regKey = "HKLM:\Software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
    $regProperty = "MSBuildToolsPath"

    $msbuildExe = Join-Path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"

    $msBuildCommand = "& ""$msbuildExe"""
    $msBuildCommand += " ""/nologo"""

    if ($properties -ne $null) {
        if ($properties.length -gt 0) {
            
            $properties.Keys | % {
                $msBuildCommand += " ""/p:$($_)=$($properties.Item($_))"""
            }
        }
    }

    if ([string]::IsNullOrWhiteSpace($toolsVersion) -eq $false) {
        $msBuildCommand += " ""/tv:$toolsVersion"""
    }

    if ([string]::IsNullOrWhiteSpace($verbosity) -eq $false) {
        $msBuildCommand += " ""/v:$verbosity"""
    }

    if ([string]::IsNullOrWhiteSpace($ignoreProjectExtensions) -eq $false) {
        $msBuildCommand += " ""/ignore:$ignoreProjectExtensions"""
    }

	if (![string]::IsNullOrWhiteSpace($target)) {
		$msBuildCommand += " ""/T:$target"""
	}
	
	$projectFileBase = [IO.Path]::GetFileNameWithoutExtension($projectFile)
	$logFile = "$($projectFileBase).log"
	
	$msBuildCommand += " ""/l:FileLogger,Microsoft.Build.Engine;logfile=$logFile"""

	<#
	if ($powerdelivery.onServer) {
		$outDir = Join-Path $currentDirectory "Binaries\$projectFileBase"
		$msBuildCommand += " ""/p:TeamBuildOutDir=$outDir"""		
	}
	else {
		$outDir = Join-Path $dropLocation "Binaries\$projectFileBase"
		$msBuildCommand += " ""/p:OutDir=$outDir"""
	}
	#>

    $msBuildCommand += " ""$projectFile"""

	Write-Host
    Write-Host "Compiling MSBuild Project:"
	Write-Host
	
    "Project File: $projectFile"
    "Configuration: $buildConfiguration"
    "Platform(s): $flavor"
    "Build .NET Version: $dotNetVersion"

    if (![string]::IsNullOrWhiteSpace($ignoreProjectExtensions)) {
        "Ignoring Extensions: $ignoreProjectExtensions"
    }

    if (![string]::IsNullOrWhiteSpace($toolsVersion)) {
        "Tools Version: $toolsVersion"
    }

	if (![string]::IsNullOrWhiteSpace($target)) {
		"Target: $target"
	}
	
	$tableFormat = @{Expression={$_.Key};Label="Key";Width=50}, `
                   @{Expression={$_.Value};Label="Value";Width=75}

    if ($properties -ne $null) {
        if ($properties.length -gt 0) {
            "Build Properties:"
			$properties | Format-Table $tableFormat -HideTableHeaders
        }
    }
	
	$currentDirectory = Get-Location

    
    Invoke-Expression $msBuildCommand
}