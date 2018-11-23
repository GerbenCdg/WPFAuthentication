using IAM.OpenIdConnect;
using IAM.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IAM.OAuth2.Events;

namespace WpfAuth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        ///// <summary>
        ///// The secret of our client (Application).
        ///// </summary>
        private const string ClientSecret = "c27e833f-e12d-462a-a761-384be5551713"; /*"3f6NggMbPtrmIBpgx-MK2xXK";*/

        ///// <summary>
        ///// The ID of our client (Application).
        ///// </summary>
        private const string ClientId = "webforms-auth"; /*"581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";*/

        ///// <summary>
        ///// The discovery URL of our OpenIdConnect config containing the endpoints
        ///// </summary>
        private const string KeycloakOIDCDiscoveryUrl = "http://10.136.11.214:8080/auth/realms/Realm1/.well-known/openid-configuration";
        private const string GoogleOIDCDiscoveryUrl = "https://accounts.google.com/.well-known/openid-configuration";

        private readonly string RedirectUri = string.Format("http://{0}:{1}/", "10.136.11.174", "4242");
        // private readonly string RedirectUriGoogle = "127.0.0.1:4242";

        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        const string logoutEndpoint = "https://oauth2.googleapis.com/revoke";

        private OIDCManager OIDCManager { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            var OAuth2Client = new OAuth2Client(ClientId, ClientSecret, RedirectUri);        
            OIDCManager = new OIDCManager(OAuth2Client, OIDCEndPoints.FromDiscoveryUrl(KeycloakOIDCDiscoveryUrl));

            AuthBrowser.OAuth2Manager = OIDCManager;
        }

        
        private async void RequestUserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var jsonResponse = await OIDCManager.RequestUserInfo();
            UserInfoLabel.Content = jsonResponse.ToString();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfoLabel.Content = "";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            UserInfoLabel.Content = "";
            OIDCManager.Logout();
        }
    }
}
