using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OpenIdConnect
{
    /// <summary>
    /// A class with public static OpenId Connect constants
    /// </summary>
    public static class OIDCConstants
    {
        public const string AuthorizationEndPoint = "authorization_endpoint";
        public const string TokenEndPoint = "token_endpoint";
        public const string TokenIntrospectEndPoint = "token_introspection_endpoint";
        public const string UserInfoEndPoint = "userinfo_endpoint";
        public const string EndSessionEndPoint = "end_session_endpoint";
        public const string RevocationEndPoint = "revocation_endpoint";


        public static class Errors
        {
            public const string InvalidToken = "invalid_token";
        }
    }
}
