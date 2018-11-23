using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OAuth2
{
    /// <summary>
    /// A class which contains OAuth2.0 endpoints, used for handling authorization of a client.
    /// </summary>
    public class OAuth2EndPoints
    {
        /// <summary>
        /// The authorization endpoint performs authentication of the end-user. This is done by redirecting the user agent to this endpoint.
        /// See : http://openid.net/specs/openid-connect-core-1_0.html#AuthorizationEndpoint
        /// </summary>
        public string Authorization { get; }

        /// <summary>
        /// The token endpoint is used to obtain tokens. Tokens can either be obtained by exchanging an authorization code or by supplying
        /// credentials directly depending on what flow is used. The token endpoint is also used to obtain new access tokens when they expire.
        /// See : http://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
        /// </summary>
        public string Token { get; }

        public OAuth2EndPoints(string authorization, string token)
        {
            Authorization = authorization;
            Token = token;
        }
         
    }
}
