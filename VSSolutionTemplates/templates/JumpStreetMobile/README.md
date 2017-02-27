# What is JumpStreetMobile?
JumpStreetMobile is an application accelerator for Azure App Service Mobile Apps.

The accelerator is captured in a Visual Studio project template that you can use to create a full-featured cross-platform 
Azure mobile app that features:
* Traditional "File | New Project" approach for an entirely Visual Studio creation experience, no Azure portal creation or 
configuration required
* A ready to run app with all these key mobile app capabilities completely integrated:
	* Cross-platform client apps (iOS, Android, Windows UWP, Win 8.1, Win 8.1 Phone)
	* Offline data sync with conflict resolution
	* Modern authentication (Facebook, Twitter, Microsoft, Google, Azure Active Directory)
	* Push notifications (Apple, Google, Windows -- Coming soon: Amazon, Baidu (Android China))
	* ARM Template that makes your solution DevOps ready and service creation can be done in Visual Studio
* Mobile application capabilities can be turned on or off individually by simple configuration rather than
requiring a coding exercise
	* Offline Sync
	* Authentication
	* Push Notifications

Unlike the current mobile app Quickstart in the Azure portal, with JumpStreetMobile there is no need to reinvent the wheel 
for each new mobile app or perform tedious and error-prone copy & paste operations from the Azure Mobile app documentation.
 
You can use JumpStreetMobile as a starting point for your next mobile application or as a sample app that shows how all the 
major features of Azure App Service Mobile Apps are integrated.

Want to see it in action before you install it?  Take a look at these [demo videos here](#Videos).

# How To Try It
You don't have to clone this repo or study its code to quickly try JumpStreetMobile. All you have to do is download and install 
the latest version by clicking the "VSSolutionTemplates.vsix" link from 
[here](https://ci.appveyor.com/project/sayedihashimi/vssolutiontemplates/build/artifacts).  Note: Once it's downloaded you may
have to rename the installer from "VSSolutionTemplates.zip" to "VSSolutionTemplates.vsix" and then simply double-click it to
get it installed in Visual Studio.  

To get a quick overview of JumpStreetMobile and see how easy it is to get started and use it, take a look at these
[demo videos here](#Videos).

**IMPORTANT:** If you don't already have the JumpStreetMobile prerequisites listed below installed, you'll need to install
them in order to successfully build the solution.

### JumpStreeMobile Prerequisites
* Visual Studio 2015 Community Update 3 or Enterprise Update 3 (VS 2017 may work but hasn't been tested)
* Visual Studio Tools for Windows Universal 
* Install SQLite runtime for Windows clients
	* Win RT 8.1
		* http://www.sqlite.org/2016/sqlite-winrt81-3120200.vsix 
	* WP 8.1
		* http://www.sqlite.org/2016/sqlite-wp81-winrt-3120200.vsix
	* UWP
		* http://sqlite.org/2016/sqlite-uwp-3120200.vsix
* Update any broken library references in the Windows projects: <br/>
	**Note:** If you see broken references to the platform specific SQLite library in the Windows projects
	(they show up yellow), then you need to update those references to point to the local libraries by 
	right-clicking References > Add Reference in each Windows project and expand the 
	"Windows folder > Extensions" tab. Then make sure the checkbox is checked for the appropriate 
	SQLite for Windows SDK along with the Visual C++ 2013 Runtime for Windows SDK. The SQLite SDK names 
	vary slightly with each Windows platform.

	If you have broken references and don't fix them as described above, the app with throw an exception after launch 
	that says:
		* "Unable to set temporary directory." and if you look at the inner exception you will see it say: "InnerException
		 = {"Unable to load DLL 'sqlite3.dll': The specified module could not be found. (Exception from HRESULT: 0x8007007E)"}"

# <a name="Videos"></a>Video
Watch this [quick 8-minute video](http://aka.ms/JSMQuickDemo) to understanding what JumpStreetMobile is and how easy it is to use.

Note: This video does skip over the configuration of the dev portals for authentication and push notification providers for brevity sake but
the documentation for configuring those dev portals is [here](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-users 
).  Once you configure the dev portals you can follow the instructions in azuredeploy.parameters.json and use the credentials you created in the
dev portals to configure the ARM template.  If there is interest, I can post a video of the click-through for that entire process.

<!---
# <a name="Videos"></a>Video
Watch this [quick 8-minute video](http://aka.ms/JSMQuickDemo) to understanding what JumpStreetMobile is and how easy it is to use.
This video does skip over the configuration of dev portals for authentication and push notification providers for brevity sake but
you can see that in the full click-through video below.

### Installation and Basic Use
Watch [this short video](http://myshortvid) to see just how easy it is to install JumpStreetMobile and F5-run it straight away.

### Full click-through of Everything
Watch [this much longer video](http://mylongervid) to see a full click-through of every step, from creation to deployment.
It covers how to register your app with the dev portals for authentication and push notification providers (Facebook, 
Twitter, Google, Microsoft, Apple, etc.) and the small details in Visual Studio that can hang up the beginner.
--->

# Footnote
JumpStreetMobile is built on top of another open source project called [pecan-waffle](https://github.com/ligershark/pecan-waffle).
Pecan-waffle a self-contained command line utility that can be used to easily create and share Visual Studio project templates
and item templates.  It allows you to take an arbitrary Visual Studio project and turn it into a Visual Studio project template
without the need to make any special modifications to the source project.  

In contrast to pecan-waffle, the standard approach for creating project templates requires you to modify the project which breaks
it so you can no long run or test that code base. The whole idea of pecan-waffle is that you can keep your source project in whatever
format you need to develop and test it and then use pecan-waffle to transform the source project into an actual Visual Studio
project template.

While pecan-waffle's approach is a significant benefit for a single project solution, its an absolute necessity for large grained
solutions like JumpStreetMobile due to it's multiple sub-projects and their tight integration with one another.  As a practical 
matter, I would not have been able to build JumpStreetMobile without pecan-waffle.

JumpStreetMobile is the first example of a VSSoluitonTemplate and I hope to build more in the future and include them
in the parent project [VSSolutionTemplates](https://github.com/VSSolutionTemplates/VSSolutionTemplates)
