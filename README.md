PowerDeploy
===========

This is a simple open source deployment utility which helps every developer to make deployments as easy as possible. The target is to support 
* IIS
* Windows Service
* XCopy
* Databases
* [you name it]

It will be easy to add your own deployment logic for another package type.

PowerDeploy has multiple components:

##### PowerDeploy.Server
REST service implemented with Servicestack to access information about deployments or to trigger deployments

##### PowerDeploy.Dashboard
What is deployed on UAT? What's in TEST? Those questions can be answered with the PowerDeploy Dashboard! We strongly believe that deployments and build's should be decoupled. While the build server creates NuGet Packages, the dashboard is used to show what packages are available you are able to trigger a deployment.

##### PowerDeploy.TemplateEngine
This is a simple string-replace mechanism which lets you transform any files (like web- or app.configs to a specific environment. For me, config transformation should really be very simple and straight forward: have a template with placeholders and do some string replacements.

There is also an integration for Visual Studio: `Install-Package PowerDeploy.PackageManagerExtension` TODO: write more
  * [Why another template engine](https://github.com/tobiaszuercher/powerdeploy/wiki)
  * [Syntax](https://github.com/tobiaszuercher/powerdeploy/wiki/Syntax)
  * [Template Engine: Best practices](https://github.com/tobiaszuercher/PowerDeploy/wiki/Template-Engine-Best-Practices)

See more in the [Wiki](https://github.com/tobiaszuercher/powerdeploy/wiki)

-----

## Core Team
 - [tobiaszuercher](https://github.com/tobiaszuercher) (Tobias Zürcher)
 - [olibanjoli](https://github.com/olibanjoli) (Oliver Zürcher)