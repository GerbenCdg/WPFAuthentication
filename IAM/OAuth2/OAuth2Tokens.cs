namespace IAM.OAuth2
{
    /// <summary>
    /// A wrapper class for OAuth2.0 Tokens (Access & Refresh token).
    /// </summary>
    public class OAuth2Tokens
    {
        /// <summary>
        /// The OAuth2.0 Access token.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// The OAuth2.0 Refresh token
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>
        /// Create a new OAuth2.0 tokens instance.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        public OAuth2Tokens(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}