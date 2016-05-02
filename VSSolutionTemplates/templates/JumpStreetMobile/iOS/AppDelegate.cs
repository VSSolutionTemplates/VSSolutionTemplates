using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;
using JumpStreetMobile.Shared.Model;
using System.Threading.Tasks;
using JumpStreetMobile.Shared.Utils;

namespace JumpStreetMobile.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            // IMPORTANT: uncomment this code to enable sync on Xamarin.iOS
            // For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
            SQLitePCL.CurrentPlatform.Init();

            LoadApplication(new JumpStreetMobile.XForms.App());

			return base.FinishedLaunching (app, options);
		}
    }
}

