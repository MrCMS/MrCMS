using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using MrCMS.Settings;
using Owin;
using Owin.Security.Providers.LinkedIn;

namespace MrCMS.Services
{
    public class AuthConfigurationService : IAuthConfigurationService
    {
        private readonly ThirdPartyAuthSettings _thirdPartyAuthSettings;

        public AuthConfigurationService(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            _thirdPartyAuthSettings = thirdPartyAuthSettings;
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
                                            {
                                                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                                                LoginPath = new PathString("/login")
                                            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            if (_thirdPartyAuthSettings.LinkedInEnabled)
            {
                app.UseLinkedInAuthentication(clientId: _thirdPartyAuthSettings.LinkedInClientId,
                                              clientSecret: _thirdPartyAuthSettings.LinkedInClientSecret);
            }


            if (_thirdPartyAuthSettings.FacebookEnabled)
            {
                var facebookAuthenticationOptions = new FacebookAuthenticationOptions
                                                        {
                                                            AppId = _thirdPartyAuthSettings.FacebookAppId,
                                                            AppSecret = _thirdPartyAuthSettings.FacebookAppSecret,
                                                        };
                facebookAuthenticationOptions.Scope.Add("email");
                app.UseFacebookAuthentication(facebookAuthenticationOptions);
            }
            if (_thirdPartyAuthSettings.GoogleEnabled)
            {
                app.UseGoogleAuthentication();
            }

        }
    }
}