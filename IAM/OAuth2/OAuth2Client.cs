using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.OAuth2
{
    /// <summary>
    /// Encapsulates information about an OAuth2 client
    /// </summary>
    public class OAuth2Client
    {
        /// <summary>
        /// The ID of the OAuth2 client.
        /// </summary>
        internal string Id { get; }

        /// <summary>
        /// The secret of the OAuth2 client.
        /// </summary>
        internal string Secret { get; }

        /// <summary>
        /// The redirect URI used by OAuth2.0, to send the 'authorization code' (also named 'request token). This should be a local IP (either localhost or your public IP), so that the Authorization code can be received by the OAuth2Manager with a HttpListener.</param>
        /// </summary>
        public string RedirectionUri { get; }

        /// <param name="clientId">The ID of the client</param>
        /// <param name="clientSecret">The secret of the client</param>
        /// <param name="oAuth2RedirectionUri">The redirect URI used by OAuth2.0, to send the 'authorization code' (also named 'request token). This should be a local IP (either localhost or your public IP), so that the Authorization code can be received by the OAuth2Manager with a HttpListener.</param>
        public OAuth2Client(string clientId, string clientSecret, string oAuth2RedirectionUri)
        {
            Id = clientId;
            Secret = clientSecret;
            RedirectionUri = oAuth2RedirectionUri;
        }
    }
}
