using JumpStreetMobile.Core.Model;
using JumpStreetMobile.Core.Utils;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace WinApp
{
    public sealed partial class MainPage : IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        async public Task<bool> Authenticate()
        {
            var success = false;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                if (user == null)
                {
                    user = await Locator.Instance.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);

                    if (user != null)
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(
                                string.Format("you are now logged in - {0}", user.UserId), "Authentication");

                        await messageDialog.ShowAsync();
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Authentication Failed");
                await messageDialog.ShowAsync();
            }

            return success;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Locator.Instance.Authenticator = this;

            base.OnNavigatedTo(e);
        }
    }
}
