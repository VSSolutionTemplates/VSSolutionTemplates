#if WINDOWS_PHONE_APP || WINDOWS_APP || XBOX

using JumpStreetMobile.Shared.Model;
using JumpStreetMobile.Shared.Utils;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml.Navigation;

#if WINDOWS_APP
namespace WinApp
#elif WINDOWS_PHONE_APP || WINDOWS_APP || XBOX
namespace WinPhone81
#endif
{
    public sealed partial class MainPage : IAuthenticate, IPushNotifications
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Locator.Instance.Authenticator = this;
            Locator.Instance.Notifier = this;

            base.OnNavigatedTo(e);
        }

        async public Task<bool> Authenticate(string provider)
        {
            var success = false;

            try
            {
                // Sign in with provider specific login using a server-managed flow
                user = await Locator.Instance.MobileService.LoginAsync(ApplicationCapabilities.ConvertString2IdentityProvider(provider));

                if (user != null)
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                //var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Authentication Failed");
                //await messageDialog.ShowAsync();
            }

            return success;
        }

        public async Task InitializeNotificationsAsync()
        {
            try
            {
                // If already registered, then just return so that we only register for push notications once
                if (Locator.Instance.IsPushNotificationsRegistered)
                    return;

                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                const string templateBodyWNS = "<toast><visual><binding template=\"ToastText01\"><text id=\"1\">$(messageParam)</text></binding></visual></toast>";

                JObject headers = new JObject();
                headers["X-WNS-Type"] = "wns/toast";

                JObject templates = new JObject();
                templates["genericMessage"] = new JObject
                {
                  {"body", templateBodyWNS},
                  {"headers", headers} // Only needed for WNS & MPNS
                };

                channel.PushNotificationReceived += OnPushNotificationReceived;

                await Locator.Instance.MobileService.GetPush().RegisterAsync(channel.Uri, templates);

                Locator.Instance.IsPushNotificationsRegistered = true;
            }
            catch (Exception)
            {
                Locator.Instance.IsPushNotificationsRegistered = false;
            }
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Push notification received: " + e.ToastNotification.Content.InnerText.Trim());

            if (Locator.Instance.IsSyncEnabled)
                await Locator.Instance.SyncChanges();
        }
    }
}

#endif