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

#### PowerDeploy.Server
REST service implemented with Servicestack and servers an AngularJS app to deploy packages and see which enviornments have which packages deployed.

#### PowerDeploy.TemplateEngine
This is a simple string-replace mechanism which lets you transform any files (like web- or app.configs to a specific environment. For me, config transformation should really be very simple and straight forward: have a template with placeholders and do some string replacements.

There is also an integration for Visual Studio: `Install-Package PowerDeploy.PackageManagerExtension` TODO: write more
  * [Why another template engine](https://github.com/tobiaszuercher/powerdeploy/wiki)
  * [Syntax](https://github.com/tobiaszuercher/powerdeploy/wiki/Syntax)
  * [[Template-Engine-Best-Practices]]


See more in the [Wiki](https://github.com/tobiaszuercher/powerdeploy/wiki)