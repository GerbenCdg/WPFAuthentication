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

namespace WpfAuth
{
    /// <summary>
    /// Interaction logic for AuthenticationBrowser.xaml
    /// </summary>
    public partial class AuthenticationBrowser : UserControl
    {
        private OAuth2Manager _OAuth2Manager;
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
            Visibility = Visibility.Hidden;
        }

        private void OnAuthorized()
        {
            Visibility = Visibility.Hidden;
        }

        private void OnAuthorizationRequired(IAM.OAuth2.Events.AuthorizationRequiredEventArgs e)
        {
            Browser.Navigate(e.DefaultAuthorizationUrl);
            Visibility = Visibility.Visible;
        }


    }
}
