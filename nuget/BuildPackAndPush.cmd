SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

DEL ..\src\PowerDeploy.PackageManagerExtension\*.nupkg

REM %NUGET% pack ..\src\PowerDeploy.PackageManagerExtension\PowerDeploy.PackageManagerExtension.nuspec

%MSBUILD% ..\src\PowerDeploy.sln /t:Rebuild /p:Configuration=Package /p:OctoPackPublishPackageToFileShare=C:\temp\nuget.server /p:OctoPackPackageVersion=0.0.3.19

REM COPY ..\src\PowerDeploy.PackageManagerExtension\src\*.nupkg C:\temp\nuget.server