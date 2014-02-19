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