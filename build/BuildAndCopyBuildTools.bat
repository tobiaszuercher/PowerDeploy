CALL "%VS120COMNTOOLS%..\..\VC\vcvarsall.bat"

msbuild PowerDeploy.MsBuild.targets /target:CopyToToolsLibFolder /v:d