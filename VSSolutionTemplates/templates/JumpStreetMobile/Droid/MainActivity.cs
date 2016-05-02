using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using JumpStreetMobile.Shared.Model;
using JumpStreetMobile.Shared.Utils;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace JumpStreetMobile.Droid
{
	[Activity (Label = "JumpStreetMobile.Droid", 
		Icon = "@drawable/icon", 
		MainLauncher = true, 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Theme = "@android:style/Theme.Holo.Light")]
	public partial class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            LoadApplication(new JumpStreetMobile.XForms.App());
		}
    }
}

