using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OAuth2
{
    /// <summary>
    /// A class with public static OAuth2 constants
    /// </summary>
    public static class OAuth2Constants
    {
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";

        public const string Code = "code";
        public const string Token = "token";

        public const string AuthorizationCode = "authorization_code";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string GrantType = "grant_type";
        public const string RedirectUri = "redirect_uri";

        public const string Error = "error";
        public const string ErrorDescription = "error_description";

        /// <summary>
        /// The possible OAuth 2.0 error responses as defined in RFC 6749
        /// See : https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>

        public static class Errors
        {
            public const string InvalidRequest = "invalid_request";
            public const string InvalidClient = "invalid_client";
            public const string InvalidGrant = "invalid_grant";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string UnsupportedGrantType = "unsupported_grant_type";
            public const string InvalidScope = "invalid_scope";
        }
    }
}
