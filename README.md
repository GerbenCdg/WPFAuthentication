# WpfOAuth2

This project contains 3 libraries : IAM, WpfIAM and WpfIAMTest.

IAM is a C# library which contains classes helping you to manage OAuth2.0 authorization, and supports OpenIdConnect aswell.
It allows you to perform authorized GET requests with an access token via the OIDCManager (OpenIdConnect Manager), which is ensured to always be valid.

WpfIAM is a WPF library containing a UserControl named 'AuthenticationBrowser', whichs shows up when your user needs to login so the OAuth2Manager can get an up-to-date access token. It is also possible to make your own component to do this.

WpfIAMTest uses this AuthenticationBrowser and shows how to use the two other libraries.

### How to use this
