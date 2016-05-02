using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Security.Claims;
using Microsoft.Azure.Mobile.Server.Authentication;
using System.Threading.Tasks;
using System.Security.Principal;
using System;

namespace JumpStreetMobileService.Controllers
{
    [MobileAppController]
    public class UserProfileController : ApiController
    {
        /// <summary>
        /// GET api/UserProfile/UserName/<provider>
        /// </summary>
        /// <param name="provider">The MobileServiceAuthenticationProvider value that specifies which provider to access</param>
        /// <returns>Returns the user's display name</returns>
        /// <remarks>
        /// If you need other properties from the provider then add additional API to this controller
        /// </remarks>
        async public Task<string> GetUserName(string provider)
        {
            string userName = null;

            try
            {
                if (provider == "MicrosoftAccount")
                {
                    MicrosoftAccountCredentials credential = await User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);

                    userName = credential.Claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
                }
                else if (provider == "Google")
                {
                    GoogleCredentials credential = await User.GetAppServiceIdentityAsync<GoogleCredentials>(Request);

                    userName = credential.Claims["name"];
                }
                else if (provider == "Twitter")
                {
                    TwitterCredentials credential = await User.GetAppServiceIdentityAsync<TwitterCredentials>(Request);

                    userName = credential.Claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
                }
                else if (provider == "Facebook")
                {
                    FacebookCredentials credential = await User.GetAppServiceIdentityAsync<FacebookCredentials>(Request);

                    userName = credential.Claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
                }
                else if (provider == "WindowsAzureActiveDirectory")
                {
                    AzureActiveDirectoryCredentials credential = await User.GetAppServiceIdentityAsync<AzureActiveDirectoryCredentials>(Request);

                    userName = credential.Claims["name"];
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in UserProfileController.GetUserName(): " + e.Message);
                userName = "<User Name Unavailable>";
            }

            return userName;
        }
    }
}
