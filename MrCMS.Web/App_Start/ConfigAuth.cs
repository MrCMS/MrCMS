using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Owin.Security.Providers.LinkedIn;

namespace MrCMS.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
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

            //app.UseLinkedInAuthentication(clientId: "", clientSecret: "");


            //var facebookAuthenticationOptions = new FacebookAuthenticationOptions
            //                                        {
            //                                            AppId = "",
            //                                            AppSecret = "",
            //                                        };
            //facebookAuthenticationOptions.Scope.Add("email");
            //app.UseFacebookAuthentication(facebookAuthenticationOptions);

            app.UseGoogleAuthentication();
        }
    }
}