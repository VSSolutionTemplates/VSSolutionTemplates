using GalaSoft.MvvmLight.Messaging;
using JumpStreetMobile.Shared.Messages;
using JumpStreetMobile.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace JumpStreetMobile.XForms.View
{
	public partial class ProviderLoginView : ContentView
	{
		public ProviderLoginView ()
		{
            InitializeComponent();

            ProviderListView.ItemsSource = ApplicationCapabilities.IdentityProviders;
        }

        async void OnAuthenticationProviderSelected(object sender, EventArgs args)
        {
            Locator.Instance.IsBusy = true;

            try
            {
                if (ProviderListView.SelectedItem != null)
                {
                    await Locator.Instance.Login((string)ProviderListView.SelectedItem);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ProviderListView.SelectedItem = null;

                        Messenger.Default.Send<InitializePage>(new InitializePage());
                    });
                }
            }
            finally
            {
                Locator.Instance.IsBusy = false;

                // Dismiss the login dialog
                Locator.Instance.IsLoginRequested = false;
            }
        }

    }
}
