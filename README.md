PowerDeploy
===========
![CI status](https://ci.appveyor.com/api/projects/status/frmeya6j84r4c8o3/branch/master)     ![NuGet Downloads](http://img.shields.io/nuget/dt/PowerDeploy.PackageManagerExtension.svg)     ![NuGet Version](http://img.shields.io/nuget/v/PowerDeploy.PackageManagerExtension.svg)

This is a simple open source deployment utility which helps every developer to make deployments as easy as possible. The target is to support 
* IIS
* Windows Service
* XCopy
* Databases
* [you name it]

It will be easy to add your own deployment logic for another package type.

PowerDeploy has multiple components:

#### PowerDeploy.Dashboard
What is deployed on UAT? What's in TEST? Those questions can be answered with the PowerDeploy Dashboard! We strongly believe that deployments and build's should be decoupled. While the build server creates NuGet Packages, the dashboard is used to show what packages are available you are able to trigger a deployment.
*Please Note: this is more or less an educational project about AngularJS, Servicestack and RavenDB for me, so nothing read for use.*

#### PowerDeploy.Transformer
Got tired of ms config-transform? Transformer is a simple template engine (based on string-replacements) which lets you transform any files (like `web.config` or `app.configs` to a specific environment. For us, config transformation should really be very simple and readable: have a template with placeholders and do some string replacements.

There is a NuGet Package which adds some commands to your NuGet Package Manager Console in Visual Studio to do the transformations locally: `Install-Package PowerDeploy.PackageManagerExtension`

#### Some links for about Transformer:
  * [Why another template engine](https://github.com/tobiaszuercher/PowerDeploy/wiki/Why-another-template-engine)
  * [Syntax](https://github.com/tobiaszuercher/powerdeploy/wiki/Syntax)
  * [Template Engine: Best practices](https://github.com/tobiaszuercher/PowerDeploy/wiki/Template-Engine-Best-Practices)

See more in the [Wiki](https://github.com/tobiaszuercher/powerdeploy/wiki)

-----

## Core Team
 - [tobiaszuercher](https://github.com/tobiaszuercher) (Tobias Zürcher)
 - [olibanjoli](https://github.com/olibanjoli) (Oliver Zürcher)

aka the twinZ
