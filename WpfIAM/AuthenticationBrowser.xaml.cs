using IAM.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Net.Http;
using System.Web;
using System.Collections.Specialized;

namespace WpfIAM
{
    /// <summary>
    /// Interaction logic for AuthenticationBrowser.xaml
    /// </summary>
    public partial class AuthenticationBrowser : UserControl
    {
        private OAuth2Manager _OAuth2Manager;
        private SemaphoreSlim _ResponseQueryStringSemaphore = new SemaphoreSlim(0, 1);
        private NameValueCollection _RedirectionQueryString;

        public OAuth2Manager OAuth2Manager
        {
            get { return _OAuth2Manager; }
            set
            {
                _OAuth2Manager = value;
                value.OnAuthorized = OnAuthorized;
                value.OnAuthorizationRequired = OnAuthorizationRequired;
            }
        }

        public AuthenticationBrowser()
        {
            InitializeComponent();
            Browser.Navigating += Browser_Navigating;
            Visibility = Visibility.Hidden;
        }

        private void OnAuthorized()
        {
            Visibility = Visibility.Hidden;
        }

        private async Task OnAuthorizationRequired(AuthorizationRequiredParameters e)
        {
            Browser.Navigate(e.DefaultAuthorizationUrl);
            Visibility = Visibility.Visible;
            
            await _ResponseQueryStringSemaphore.WaitAsync();
            e.RedirectionUriParameters = _RedirectionQueryString;
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.Query.Contains($"&{OAuth2Constants.Code}="))
            {
                _RedirectionQueryString = ParseQueryString(e.Uri.Query);
                _ResponseQueryStringSemaphore.Release();
            }
        }

        /// <summary>
        /// Parse a query string
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns>The queryString as key/value pairs</returns>
        protected NameValueCollection ParseQueryString(string queryString)
        {
            NameValueCollection queryParameters = new NameValueCollection();
            string[] querySegments = queryString.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }
            return queryParameters;
        }
    }
}
