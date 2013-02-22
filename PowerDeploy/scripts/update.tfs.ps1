# too lazy atm... WHY THE FUCK IS THERE NO CLEAN COMMANDLINE TOOL TO ACCESS TFS!!! CMDLETS, SnapIns, tf.exe 
# WHY THE FUCK ARE THERE SO MANY WAY?????!!!!!!
Set-Alias tf "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"

Set-Location $powerdeploy.paths.project

cd /d c:\
if not exist %TARGET% (
  mkdir %TARGET%
  cd /d %TARGET%
  tf workfold /unmap %TARGET% > nul
  tf workspace /new /server:http://zrhtfs02:8080/tfs/projects /noprompt NP
  tf workfold %SCRIPTSPATH% %TARGET% /workspace:NP
)
cd /d %TARGET%
tf get %TARGET% /recursive /overwrite /noprompt