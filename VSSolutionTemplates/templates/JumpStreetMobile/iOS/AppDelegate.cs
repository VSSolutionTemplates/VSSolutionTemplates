using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using UIKit;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;
using JumpStreetMobile.Model;
using System.Threading.Tasks;
using JumpStreetMobile.Utils;

namespace JumpStreetMobile.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate, IPushNotifications
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize Azure Mobile Apps
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            // Initialize Xamarin Forms
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            Locator.Instance.Authenticator = this;
            Locator.Instance.Notifier = this;

            base.OnActivated(uiApplication);
        }

        public async Task InitializeNotificationsAsync()
        {
            // registers for push for iOS8
            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

            JObject templates = new JObject();
            templates["genericMessage"] = new JObject
                {
                  {"body", templateBodyAPNS}
                };

            // Register for push with your mobile app
            Push push = Locator.Instance.MobileService.GetPush();
            push.RegisterAsync(deviceToken, templates);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();

            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
            }
        }

        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate(string provider)
        {
            var success = false;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                if (user == null)
                {
                    user = await Locator.Instance.MobileService.LoginAsync(
                        UIApplication.SharedApplication.KeyWindow.RootViewController,
                        ApplicationCapabilities.ConvertString2IdentityProvider(provider));

                    //if (user != null)
                    //{
                    //    UIAlertView avAlert = new UIAlertView("Authentication", "You are now logged in " + user.UserId, null, "OK", null);
                    //    avAlert.Show();
                    //}
                }

                success = true;
            }
            catch (Exception ex)
            {
                //UIAlertView avAlert = new UIAlertView("Authentication failed", ex.Message, null, "OK", null);
                //avAlert.Show();
            }

            return success;
        }
    }
}

