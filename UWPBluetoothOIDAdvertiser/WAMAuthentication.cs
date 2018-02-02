using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;

namespace UWPBluetoothOIDAdvertiser
{
    public class WAMAuthentication
    {
        const string DefaultProviderId = "https://login.windows.local";
        const string AzureActiveDirectoryAuthority = "organizations";
        const string AzureActiveDirectoryClientId = "4b9baed6-f487-455a-9897-a64a04b46504";

        private WAMAuthentication()
        {
        }

        private static WAMAuthentication instance = new WAMAuthentication();
        public static WAMAuthentication Instance
        {
            get
            {
                return instance;
            }
        }

        public string AccessToken
        {
            get;
            private set;
        }

        public string TenantId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public async Task AuthenticateDefaultUser()
        {
            var defaultProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(DefaultProviderId);

            if (defaultProvider != null)
            {
                await AuthenticateWithRequestToken(defaultProvider, string.Empty, AzureActiveDirectoryClientId);
            }
        }

        private async Task AuthenticateWithRequestToken(WebAccountProvider Provider, String Scope, String ClientID)
        {
            try
            {
                WebTokenRequest webTokenRequest = new WebTokenRequest(Provider, Scope, ClientID);
                webTokenRequest.Properties.Add("resource", "https://graph.windows.net");

                WebTokenRequestResult webTokenRequestResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest);

                if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    TenantId = webTokenRequestResult.ResponseData[0].Properties["tid"];
                    AccessToken = webTokenRequestResult.ResponseData[0].Token;
                    Name = webTokenRequestResult.ResponseData[0].WebAccount.UserName;
                }

            }
            catch (Exception)
            {
            }
        }
    }
}
