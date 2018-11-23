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
        /// The redirect url used by OAuth2.0, used to send the Authorization code. This should be a local IP (either localhost or your public IP), so you can retrieve the Authorization code.</param>
        /// </summary>
        public string RedirectionUrl { get; }

        /// <param name="clientId">The ID of the client</param>
        /// <param name="clientSecret">The secret of the client</param>
        public OAuth2Client(string clientId, string clientSecret, string oAuth2RedirectionUrl)
        {
            Id = clientId;
            Secret = clientSecret;
            RedirectionUrl = oAuth2RedirectionUrl;
        }
    }
}
