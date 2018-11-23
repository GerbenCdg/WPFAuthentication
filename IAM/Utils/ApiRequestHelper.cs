using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace IAM.Utils
{
    /// <summary>
    /// A class which allows to easily perform POST and GET requests.
    /// </summary>
    public class ApiRequestHelper
    {
        /// <summary>
        /// Initialize a new HTTPWebRequest instance from a url and header options. 
        /// </summary>
        /// <param name="uri">The URI that identifies the Internet resource</param>
        /// <param name="headerOptions">Options to add to the header of the request</param>
        /// <returns></returns>
        public HttpWebRequest CreateHttpWebRequest(string uri, params Tuple<HttpRequestHeader, string>[] headerOptions)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            headerOptions.ToList().ForEach(option => request.Headers.Add(option.Item1, option.Item2));

            return request;
        }

        /// <summary>
        /// Initialize a new HttpWebRequest instance from a uri and adds the accessToken to the header options of the request.
        /// </summary>
        /// <param name="uri">The URI that identifies the Internet resource</param>
        /// <param name="accessToken">The access token which provides access to the specified resource</param>
        /// <returns></returns>
        public HttpWebRequest CreateAuthorizedWebRequest(string uri, string accessToken)
        {
            return CreateHttpWebRequest(uri, Tuple.Create(HttpRequestHeader.Authorization, "Bearer " + accessToken));
        }

        /// <summary>
        /// Perform a GET request 
        /// </summary>
        /// <param name="request">The request to get a response from</param>
        /// <returns>The response of the request as a JSON object</returns>
        public JObject Get(WebRequest request)
        {
            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();
                return (JObject)JsonConvert.DeserializeObject(responseString);
            }
        }

        /// <summary>
        /// Perform an authenticated GET request 
        /// </summary>
        /// <param name="uri">The URI that identifies the Internet resource</param>
        /// <param name="accessToken">The access token as authorization proof</param>
        /// <returns>The response of the request as a JSON object</returns>
        public JObject Get(string uri, string accessToken)
        {
            return Get(CreateAuthorizedWebRequest(uri, accessToken));
        }

        /// <summary>
        /// Perform a POST request
        /// </summary>
        /// <param name="uri">The URI that identifies the Internet resoruce</param>
        /// <param name="bodyParameters">The POST request body key/value pairs</param>
        /// <returns>The Json response of the request</returns>
        public JObject Post(string uri, NameValueCollection bodyParameters)
        {
            using (var client = new WebClient())
            {
                var response = client.UploadValues(uri, bodyParameters);

                var responseString = Encoding.Default.GetString(response);
                return (JObject)JsonConvert.DeserializeObject(responseString);
            }
        }

    }
}
