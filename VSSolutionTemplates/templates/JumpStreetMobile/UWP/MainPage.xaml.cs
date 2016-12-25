using JumpStreetMobile.Model;
using JumpStreetMobile.Utils;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IAuthenticate, IPushNotifications
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new JumpStreetMobile.App("WINDOWS_UWP"));
        }

        // Define a authenticated user.
        private MobileServiceUser user;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
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