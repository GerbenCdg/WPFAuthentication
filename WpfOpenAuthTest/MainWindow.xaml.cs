using DotNetOpenAuth.OAuth2;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Net;
using DotNetOpenAuth.Messaging;
using System.IO;
using System.Threading;

namespace WpfOpenAuthTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ClientSecret = "c27e833f-e12d-462a-a761-384be5551713"; /*"3f6NggMbPtrmIBpgx-MK2xXK";*/
        private const string ClientId = "webforms-auth"; /*"581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";*/
        private const bool ImplicitGrant = true;
        private const string Scopes = "";

        private IAM.OAuth2.OAuth2EndPoints _EndPoints = IAM.OpenIdConnect.OIDCEndPoints.FromDiscoveryUrl("http://10.136.11.214:8080/auth/realms/Realm1/.well-known/openid-configuration");

        public MainWindow()
        {
            InitializeComponent();

            var authServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(_EndPoints.Authorization),
                TokenEndpoint = new Uri(_EndPoints.Token)
            };

            var client = new UserAgentClient(authServer, ClientId, ClientSecret);
            clientAuthorizationView.Host = clientAuthorizationViewHost;
            clientAuthorizationView.Client = client;

            clientAuthorizationView.Authorization.Scope.AddRange(OAuthUtilities.SplitScopes(Scopes));
            clientAuthorizationView.Authorization.Callback = new Uri(string.Format("http://{0}/", IPAddress.Loopback));
        }

        private void clientAuthorizationView_Completed(object sender, ClientAuthorizationCompleteEventArgs e)
        {
            AuthorizationSuccess = e.Authorization != null && e.Authorization.AccessToken != null;
            //clientAuthorizationViewHost.Visibility = Visibility.Hidden;
            AuthorizationGrantedSemaphore.Release();
        }

        private readonly SemaphoreSlim AuthorizationGrantedSemaphore = new SemaphoreSlim(0);
        private bool AuthorizationSuccess { get; set; }

        private async void MakeUserInfoRequest()
        {
            //var authServer = new AuthorizationServerDescription
            //{
            //    AuthorizationEndpoint = new Uri(_EndPoints.Authorization),
            //    TokenEndpoint = new Uri(_EndPoints.Token)
            //};
            clientAuthorizationViewHost.Visibility = Visibility.Visible;

            try
            {
                //var client = new UserAgentClient(authServer, ClientId, ClientSecret);

                //// var authorizePopup = new Authorize2(client);
                //clientAuthorizationView.Client = client;

                //clientAuthorizationView.Authorization.Scope.AddRange(OAuthUtilities.SplitScopes(Scopes));
                //clientAuthorizationView.Authorization.Callback = new Uri(string.Format("http://{0}/", IPAddress.Loopback));
                // clientAuthorizationView.Owner = this;
                // authorizePopup.ClientAuthorizationView.RequestImplicitGrant = ImplicitGrant; (only available in DotNetOpenAuth 5.0-beta)
                clientAuthorizationView.Reload();

                //clientAuthorizationViewHost.Visibility = Visibility.Visible;
                await AuthorizationGrantedSemaphore.WaitAsync();
                
                if (AuthorizationSuccess)
                {
                    AuthorizationSuccess = false;

                    var requestUri = new Uri((_EndPoints as IAM.OpenIdConnect.OIDCEndPoints).UserInfo);

                    var request = (HttpWebRequest)WebRequest.Create(requestUri);
                    request.Method = "GET";
                    clientAuthorizationView.Client.AuthorizeRequest(request, clientAuthorizationView.Authorization);

                    using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        string responseString = reader.ReadToEnd();
                        MessageBox.Show(responseString);
                        // var response = (JObject)JsonConvert.DeserializeObject(responseString);
                    }
                }
            }

            catch (ProtocolException ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            catch (WebException ex)
            {
                string responseText = string.Empty;
                if (ex.Response != null)
                {
                    using (var responseReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        responseText = responseReader.ReadToEnd();
                    }
                }
                MessageBox.Show(this, ex.Message + "  " + responseText);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MakeUserInfoRequest();
        }
    }
}
