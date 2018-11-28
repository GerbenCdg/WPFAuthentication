using System;
using System.Threading.Tasks;
using IAM.OAuth2;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace IAM.OpenIdConnect
{
    /// <summary>
    /// A class which extends the functionality provided by the OAuth2Manager.
    /// It allows to revoke authorization by logging out, and to request user information.
    /// </summary>
    public class OIDCManager : OAuth2.OAuth2Manager
    {
        private OIDCEndPoints EndPoints { get; set; }

        /// <summary>
        /// Create a new OIDCManager instance.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endPoints"></param>
        /// <param name="onAuthorizationRequiredHandler"></param>
        public OIDCManager(OAuth2Client client, OIDCEndPoints endPoints, Func<AuthorizationRequiredParameters, Task> onAuthorizationRequiredHandler)
            : base(client, endPoints, onAuthorizationRequiredHandler)
        {
            EndPoints = endPoints;
        }

        /// <summary>
        /// Create a new OIDCManager instance. You have to subscribe to the OnAuthorizationRequired event. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endPoints"></param>
        public OIDCManager(OAuth2Client client, OIDCEndPoints endPoints)
            : base(client, endPoints)
        {
            EndPoints = endPoints;
        }

        /// <summary>
        /// Ensures the user is logged out calling the logout or revoke endpoint (depending on the OpenID Connect implementation) specified by OpenId Connect.
        /// </summary>
        public void Logout()
        {
            if (Tokens == null) return;

            var bodyParameters = AuthorizationParameters;
            string endPoint;

            // Calls either the logout or the revocation URL (this depends on the OpenID Connect implementation)
            if (EndPoints.Logout != null)
            {
                endPoint = EndPoints.Logout;
                bodyParameters.Add(new NameValueCollection()
                {
                    [OAuth2Constants.RefreshToken] = Tokens.RefreshToken,
                });
            }
            else
            {
                endPoint = EndPoints.Revocation;
                bodyParameters.Add(new NameValueCollection()
                {
                    [OAuth2Constants.Token] = Tokens.RefreshToken,
                });
            }
            ApiRequestHelper.Post(endPoint, bodyParameters);
            ClearTokens();
        }

        /// <summary>
        /// Request user information by requesting the userInfo endpoint which is specified by OpenID Connect.
        /// </summary>
        /// <returns>A Json object with the server response.</returns>
        public async Task<JObject> RequestUserInfo()
        {
            return await PerformAuthenticatedGet(EndPoints.UserInfo);
        }


    }
}
