using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OpenIdConnect
{
    /// <summary>
    /// Class containing the essential URIs of the OpenId Connect endpoints.
    /// It is recommended to use the static method FromDiscoveryUrl(.., ..) to create a new instance of this class
    /// </summary>
    public class OIDCEndPoints : OAuth2.OAuth2EndPoints
    {
        /// <summary>
        /// The user info endpoint.
        /// </summary>
        public string UserInfo { get; }
        /// <summary>
        /// The logout endpoint.
        /// </summary>
        public string Logout { get; }
        /// <summary>
        /// The revocation endpoint.
        /// </summary>
        public string Revocation { get; }

        /// <summary>
        /// Create a new instance of an OpenIdConnect endpoint.
        /// </summary>
        /// <param name="userInfo">The userInfo endpoint</param>
        /// <param name="logout">The logout endpoint. Either this parameter or the revocation parameter may be null depending on the OpenId Connect implementation that you are using.</param>
        /// <param name="revocation">The revocation endpoint. Either this parameter or the revocation parameter may be null depending on the OpenId Connect implementation that you are using.</param>
        /// <param name="authorization">The authorization endpoint.</param>
        /// <param name="token">The token endpoint.</param>
        public OIDCEndPoints(string userInfo, string logout, string revocation, string authorization, string token) : base(authorization, token)
        {
            if (revocation == null && logout == null) throw new ArgumentNullException("Revocation and Logout endpoints cannot both be null");

            UserInfo = userInfo;
            Logout = logout;
            Revocation = revocation;
        }

        /// <summary>
        /// Helper method that creates an instance of this class from a .well-known/openid-configuration URL, and an OAuth2RedirectionURL.
        /// </summary>
        /// <param name="openIdConfigurationUrl">The .well-known/openid-configuration URL as defined by the OpenId Connect standards.</param>
        /// <returns>An instance this class.</returns>
        public static OIDCEndPoints FromDiscoveryUrl(string openIdConfigurationUrl)
        {
            var apiHelper = new Utils.ApiRequestHelper();
            var response = apiHelper.Get(apiHelper.CreateHttpWebRequest(openIdConfigurationUrl));

            string authorizationEp = response.Value<string>(OIDCConstants.AuthorizationEndPoint);
            string tokenEp = response.Value<string>(OIDCConstants.TokenEndPoint);
            string userInfoEp = response.Value<string>(OIDCConstants.UserInfoEndPoint);
            string logoutEp = response.Value<string>(OIDCConstants.EndSessionEndPoint);
            string revocationEp = response.Value<string>(OIDCConstants.RevocationEndPoint);

            return new OIDCEndPoints(userInfoEp, logoutEp, revocationEp, authorizationEp, tokenEp);
        }
    }
}
