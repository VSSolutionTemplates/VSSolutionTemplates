# What is JumpStreetMobile?
JumpStreetMobile is an application accelerator for Azure App Service Mobile Apps.

The accelerator is captured in a Visual Studio project template that you can use to create a full-featured cross platform 
Azure mobile app that features:
* Traditional "File | New Project" approach for an entirely Visual Studio creation experience, no Azure portal creation or 
configuration required
* Generated app is ready to run with all these key mobile app capabilities completely integrated:
	* Cross platform client apps (iOS, Android, Windows UWP, Win 8.1, Win 8.1 Phone)
	* Offline data sync with conflict resolution
	* Modern authentication (Facebook, Twitter, Microsoft, Google, Azure Active Directory)
	* Push notifications (Apple, Google, Windows, Amazon, Baidu (Android China))
	* ARM Template means your solution is DevOps ready and service creation can be done in Visual Studio
* Application capabilities can be turned on or off individually by simple configuration rather than requiring a coding exercise
	* Offline Sync
	* Authentication
	* Push Notifications

Unlike the current mobile app Quickstart in the Azure portal, with JumpStreetMobile there is no need to reinvent the wheel 
for each new mobile app or perform tedious and error-prone copy & paste operations from the Azure Mobile app documentation.
 
You can use JumpStreetMobile as a starting point for your next mobile application or as a sample app that shows how all the 
major features of Azure App Service Mobile Apps are integrated.

# How To Try It
You don't have to clone this repo or study its code to try JumpStreetMobile. All you have to do is get the latest .vsix
by clicking the [VSSolutionTemplates.vsix](https://ci.appveyor.com/project/sayedihashimi/vssolutiontemplates/build/artifacts) link
from [here](https://ci.appveyor.com/project/sayedihashimi/vssolutiontemplates/build/artifacts) and install the Azure Mobile app
prerequisites listed below.<br/>

**Important!!!**  The .vsix will download as a .zip so you will have to rename it to .vsix and then double-click it to
install it in Visual Studio.

## Azure Mobile App Prerequisites
List prereqs here

# Video
Watch [this short video](http://myshortvid) to see just how easy it is to build a fully featured Azure mobile app.

Watch [this much longer video](http://mylongervid) to see how to register your app with the authentication and push notification providers (Facebook,
Twitter, Google, Microsoft, Apple, etc.).  Sorry, configuring those developer portals is outside the scope of what a solution 
template can target and automate!  Someday maybe.
