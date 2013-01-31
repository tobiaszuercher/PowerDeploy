function Load-Configuration {
    param(
        [string] $configdir = $PSScriptRoot
    )

    #$psakeConfigFilePath = (join-path $configdir "psake-config.ps1")

    $env = @{}
    $env.project = @{}
    

    $env.project.dir = "123"

}