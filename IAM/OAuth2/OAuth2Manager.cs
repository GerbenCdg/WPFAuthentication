using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace IAM.OAuth2
{
    /// <summary>
    /// A class which handles authorization with OAuth 2.0.
    /// It holds access tokens and refresh tokens, and takes care of updating them.  
    /// It allows to make authorized GET requests.
    /// </summary>
    public abstract class OAuth2Manager
    {
        protected OAuth2Tokens Tokens { get; private set; }

        protected OAuth2Client Client { get; set; }
        private OAuth2EndPoints EndPoints { get; set; }

        /// <summary>
        /// A helper class to easily perform GET and POST requests (optionaly, with an authorization bearer token).
        /// </summary>
        protected Utils.ApiRequestHelper ApiRequestHelper { get; } = new Utils.ApiRequestHelper();

        /// <summary>
        /// This event is raised when the program needs to request authorization from the user.
        /// Use the AuthorizationEventArgs to build the correct URI to access with a WebBrowser.
        /// Then, display this browser to your user.
        /// 
        /// Subscription to this event is mandatory.
        /// </summary>
        public Func<AuthorizationRequiredParameters, Task> OnAuthorizationRequired { protected get; set; }

        /// <summary>
        /// Subscribe to this event to be informed when Authorization has been granted.
        /// </summary>
        public Action OnAuthorized { protected get; set; }

        /// <summary>
        /// Returns a new NameValueCollection containing your client ID and client Secret for your API requests
        /// </summary>
        protected NameValueCollection AuthorizationParameters
        {
            get
            {
                return new NameValueCollection()
                {
                    [OAuth2Constants.ClientId] = Client.Id,
                    [OAuth2Constants.ClientSecret] = Client.Secret,
                };
            }
        }

        /// <summary>
        /// Create a new OAuth2Manager instance.
        /// </summary>
        /// <param name="client">an OAuth2.0 client</param>
        /// <param name="endPoint">a OAuth2.0 endpoints wrapper</param>
        /// <param name="onAuthorizationRequiredHandler">A Handler for the OnAuthorized event, which is executed when the user of this OAuth2Manager needs to display a browser where the user is prompted to provider authorization</param>
        protected OAuth2Manager(OAuth2Client client, OAuth2EndPoints endPoint, Func<AuthorizationRequiredParameters, Task> onAuthorizationRequiredHandler)
        {
            Client = client;
            EndPoints = endPoint;
            OnAuthorizationRequired = onAuthorizationRequiredHandler;
        }

        /// <summary>
        /// Create a new OAuth2Manager instance. Don't forget to subscribe to the OnAuthorizationRequired event. 
        /// </summary>
        /// <param name="client">an OAuth2.0 client</param>
        /// <param name="endPoint">a OAuth2.0 endpoints wrapper</param>
        protected OAuth2Manager(OAuth2Client client, OAuth2EndPoints endPoint)
        {
            Client = client;
            EndPoints = endPoint;
        }

        /// <summary>
        /// Tries to request a new AccessToken from the current RefreshToken by calling the Token endpoint. 
        /// If this fails, it probably means that the RefreshToken has expired. In that case, we prompt our user for authorization again.
        /// </summary>
        protected virtual async Task RequestValidAccessToken()
        {
            if (Tokens == null || Tokens.RefreshToken == null) throw new NullReferenceException("Tokens are null. Make sure that WaitForUserAuthorization(..) has been called in order to set Tokens.");

            var bodyParameters = AuthorizationParameters;
            bodyParameters.Add(new NameValueCollection()
            {
                [OAuth2Constants.GrantType] = OAuth2Constants.RefreshToken,
                [OAuth2Constants.RefreshToken] = Tokens.RefreshToken,
            });

            try
            {
                // Try to refresh the accessToken by requesting a new one at the token endpoint, specifying the refresh token.
                var response = ApiRequestHelper.Post(EndPoints.Token, bodyParameters);
                Tokens = new OAuth2Tokens(response.Value<string>(OAuth2Constants.AccessToken), Tokens.RefreshToken);
            }
            catch (WebException e)
            {
                if (!(e.Status.Equals(WebExceptionStatus.ProtocolError) && e.Response is HttpWebResponse))
                    throw e;

                // We get more information about what went wrong
                HttpWebResponse response = (HttpWebResponse)e.Response;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var errorResponseBody = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());
                    string error = errorResponseBody.Value<string>(OAuth2Constants.Error);

                    if (error.Equals(OAuth2Constants.Errors.InvalidGrant))
                    {
                        // Most probably, the refresh token has expired
                        await WaitForUserAuthorization();
                        return;
                    }
                    string description = errorResponseBody.Value<string>(OAuth2Constants.ErrorDescription);
                    throw new WebException($"Error: {error}\nDescription: {description}");
                }
            }
        }


        /// <summary>
        /// Initializes a listener on the RedirectionUrl of our Client (which should be localhost). 
        /// Then, we invoke the handler which should display a browser to the user.
        /// The user should be able to provide authorization in this browser.
        /// </summary>
        protected virtual async Task WaitForUserAuthorization()
        {
            if (OnAuthorizationRequired == null) throw new NullReferenceException("OnAuthorizationRequired must be subscribed to ! There, you must show a web browser which shows an OAuth2.0 authorization page");

            // Raise the OnAuthorizationRequired event to which should be subscribed
            var authorizationRequiredParams = new AuthorizationRequiredParameters(EndPoints.Authorization, Client.Id, Client.RedirectionUri);
            await OnAuthorizationRequired.Invoke(authorizationRequiredParams);

            var responseParameters = authorizationRequiredParams.RedirectionUriParameters;

            OnAuthorized.Invoke();

            // The queryString of the redirect URI should contain the authorization code, but not any errors.
            if (responseParameters.Get("error") != null)
            {
                throw new WebException("OAuth authorization error: " + responseParameters.Get("error"));
            }

            var authorizationCode = responseParameters.Get("code");
            RequestTokensFromAuthorizationCode(authorizationCode);
        }

        /// <summary>
        /// Exchanges the authorizationCode for an accessToken and refreshToken.
        /// </summary>
        /// <param name="authorizationCode"></param>
        protected virtual void RequestTokensFromAuthorizationCode(string authorizationCode)
        {
            var bodyParameters = AuthorizationParameters;
            bodyParameters.Add(new NameValueCollection()
            {
                [OAuth2Constants.GrantType] = OAuth2Constants.AuthorizationCode,
                [OAuth2Constants.Code] = authorizationCode,
                [OAuth2Constants.RedirectUri] = Client.RedirectionUri
            });

            try
            {
                // Try to exchange our authorizationCode for new access and refresh tokens at the Token endpoint.
                var responseJson = ApiRequestHelper.Post(EndPoints.Token, bodyParameters);

                var accessToken = responseJson.Value<string>(OAuth2Constants.AccessToken);
                var refreshToken = responseJson.Value<string>(OAuth2Constants.RefreshToken);

                Tokens = new OAuth2Tokens(accessToken, refreshToken);
            }
            catch (WebException e)
            {
                if (e.Status.Equals(WebExceptionStatus.ProtocolError) && e.Response is HttpWebResponse)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;
                    throw GetInformativeWebException(response);
                }
                throw e;
            }
        }

        /// <summary>
        /// Reads the HttpWebResponse stream to provide an exception with more information about the error.
        /// </summary>
        /// <param name="oAuth2ErrorResponse"></param>
        /// <returns>A WebException</returns>
        protected WebException GetInformativeWebException(HttpWebResponse oAuth2ErrorResponse)
        {
            using (var reader = new StreamReader(oAuth2ErrorResponse.GetResponseStream()))
            {
                var errorResponseBody = (JObject)JsonConvert.DeserializeObject(reader.ReadToEnd());

                string error = errorResponseBody.Value<string>(OAuth2Constants.Error);
                string description = errorResponseBody.Value<string>(OAuth2Constants.ErrorDescription);

                return new WebException($"Error: {error}\nDescription: {description}");
            }
        }

        /// <summary>
        /// Make a GET request where the necessary authorization data (access token) is added to the header.
        /// If the access token is expired, it is refreshed from the refresh token if available, 
        /// otherwise the OnAuthorizationRequired event is fired.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual async Task<JObject> PerformAuthenticatedGet(string url)
        {
            if (Tokens == null)
            {
                await WaitForUserAuthorization();
            }
            try
            {
                return ApiRequestHelper.Get(url, Tokens.AccessToken);
            }
            catch (WebException e)
            {
                if (e.Status.Equals(WebExceptionStatus.ProtocolError) && e.Response is HttpWebResponse)
                {
                    HttpWebResponse errorResponse = (HttpWebResponse)e.Response;

                    if (errorResponse.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        await RequestValidAccessToken();
                        return PerformAuthenticatedGet(url).Result;
                    }
                    throw GetInformativeWebException(errorResponse);
                }
                throw e;
            }
        }

        /// <summary>
        /// Clears the tokens of this manager instance. This should be called after logging out, or after clearing the user session.
        /// </summary>
        protected void ClearTokens()
        {
            Tokens = null;
        }

    }
}
