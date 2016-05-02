#if WINDOWS_PHONE_APP

using JumpStreetMobile.Shared.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using Microsoft.WindowsAzure.MobileServices;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WinPhone81
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Authenticate on activatoin
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// This method was added because of Window Phone brokered authentication model
        /// and is necessary to successfully authenticate but it is unique to WP.
        /// </remarks>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                Locator.Instance.MobileService.LoginComplete(args as WebAuthenticationBrokerContinuationEventArgs);
            }
        }
    }
}

#endif