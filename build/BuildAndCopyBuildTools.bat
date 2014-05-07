SET MSBUILD="C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe"

%MSBUILD% PowerDeploy.MsBuild.targets /target:CopyToToolsLibFolder /v:d