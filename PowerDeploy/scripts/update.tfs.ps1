$location = Get-Location

$workspace_name = "PowerDeploy"

$local_dir = $powerdeploy.config.update.PowerDeployInstallDir
$remote_dir = $powerdeploy.config.update.source
$tfs_server = $powerdeploy.config.update.server

# too lazy atm... WHY THE FUCK IS THERE NO CLEAN COMMANDLINE TOOL TO ACCESS TFS!!! CMDLETS, SnapIns, tf.exe 
# WHY THE FUCK ARE THERE SO MANY WAYS?????!!!!!!
Set-Alias tf "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"

Set-Location $powerdeploy.paths.project

if (Test-Path $local_dir)
{
	New-Item -ItemType directory -Path $local_dir
	Set-Location $local_dir

	tf workfold /unmap $local_dir #| Out-Null
	tf workspace /new /server:$tfs_server /noprompt $workspace_name
	tf workfold $local_dir $remote_dir /workspace:$workspace_name
}

Set-Location $local_dir

tf get $local_dir /recursive /overwrite /noprompt

Set-Location $location

# cd /d c:\
# if not exist %TARGET% (
#   mkdir %TARGET%
#   cd /d %TARGET%
#   tf workfold /unmap %TARGET% > nul
#   tf workspace /new /server:http://zrhtfs02:8080/tfs/projects /noprompt NP
#   tf workfold %SCRIPTSPATH% %TARGET% /workspace:NP
# )
# cd /d %TARGET%
# tf get %TARGET% /recursive /overwrite /noprompt