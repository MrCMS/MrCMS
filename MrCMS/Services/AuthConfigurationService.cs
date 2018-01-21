using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
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
            if (_thirdPartyAuthSettings.LinkedInEnabled
                 && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.LinkedInClientId)
                 && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.LinkedInClientSecret))
            {
                app.UseLinkedInAuthentication(clientId: _thirdPartyAuthSettings.LinkedInClientId,
                    clientSecret: _thirdPartyAuthSettings.LinkedInClientSecret);
            }

            if (_thirdPartyAuthSettings.FacebookEnabled
                && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.FacebookAppId)
                && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.FacebookAppSecret))
            {
                var options = new FacebookAuthenticationOptions
                {
                    AppId = _thirdPartyAuthSettings.FacebookAppId,
                    AppSecret = _thirdPartyAuthSettings.FacebookAppSecret,
                };
                options.Fields.Add("email");
                options.Scope.Add("email");
                app.UseFacebookAuthentication(options);
            }

            if (_thirdPartyAuthSettings.GoogleEnabled 
                    && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.GoogleClientSecret)
                    && !string.IsNullOrWhiteSpace(_thirdPartyAuthSettings.GoogleClientId))
            {
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
                  {
                      ClientId = _thirdPartyAuthSettings.GoogleClientId,
                      ClientSecret = _thirdPartyAuthSettings.GoogleClientSecret,
                  });
            }
        }
    }
}