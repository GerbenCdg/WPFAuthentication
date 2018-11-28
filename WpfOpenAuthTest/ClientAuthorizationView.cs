using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace WpfOpenAuthTest
{
    class ClientAuthorizationView : DotNetOpenAuth.OAuth2.ClientAuthorizationView
    {

        public WindowsFormsHost Host{ get; set; }

        public ClientAuthorizationView()
        {
            base.HandleCreated += ClientAuthorizationView_HandleCreated;
        }

        private void ClientAuthorizationView_HandleCreated(object sender, EventArgs e)
        {
            browser = (WebBrowser) Controls[0];
            browser.Navigated += Browser_Navigated;  
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            bool shouldBeHidden = e.Url.Host.Equals(System.Net.IPAddress.Loopback.ToString());
            Host.Visibility = shouldBeHidden ? Visibility.Hidden : Visibility.Visible;
        }

        private WebBrowser browser { get; set; }

        public void Reload()
        {
            Uri authorizationUrl = this.Client.RequestUserAuthorization(this.Authorization);
            browser.Navigate(authorizationUrl.AbsoluteUri);
        }

    }
}
