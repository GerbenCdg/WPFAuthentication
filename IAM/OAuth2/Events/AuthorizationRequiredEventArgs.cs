using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OAuth2.Events
{
    /// <summary>
    /// These EventArgs provide a default AuthorizationUrl to use in your WebBrowser component. However, if this doesn't fit your needs, 
    /// use the public properties to build your own redirection URL.
    /// </summary>
    public class AuthorizationRequiredEventArgs : EventArgs
    {
        public string DefaultAuthorizationUrl
        {
            get
            {
                return AuthorizationEndPoint + $"?response_type=code&client_id={ClientId}&redirect_uri={RedirectUri}&scope=openid%20profile";
            }
        }

        public string AuthorizationEndPoint { get; }
        public string ClientId { get; }
        public string RedirectUri { get; }

        public AuthorizationRequiredEventArgs(string authorizationEp, string clientId, string redirectUri)
        {
            AuthorizationEndPoint = authorizationEp;
            ClientId = clientId;
            RedirectUri = redirectUri;
        }
    }
}
