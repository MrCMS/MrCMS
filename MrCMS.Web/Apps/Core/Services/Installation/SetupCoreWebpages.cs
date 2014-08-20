using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreWebpages : ISetupCoreWebpages
    {
        private readonly ISession _session;
        private readonly IConfigurationProvider _configurationProvider;

        public SetupCoreWebpages(ISession session, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _configurationProvider = configurationProvider;
        }

        private class ErrorPages 
        {
            public Webpage Error403 { get; set; }
            public Webpage Error404 { get; set; }
            public Webpage Error500 { get; set; }
        }

        public void Setup()
        {
            _session.Transact(session =>
            {
                GetBasicPages().ForEach(webpage => session.Save(webpage));

                ErrorPages errorPages = GetErrorPages();
                session.Save(errorPages.Error403);
                session.Save(errorPages.Error404);
                session.Save(errorPages.Error500);

                var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
                siteSettings.Error403PageId = errorPages.Error403.Id;
                siteSettings.Error404PageId = errorPages.Error404.Id;
                siteSettings.Error500PageId = errorPages.Error500.Id;
                _configurationProvider.SaveSettings(siteSettings);

                GetAccountPages().ForEach(webpage => session.Save(webpage));

                session.Save(GetSearchPage());

                var webpages = _session.QueryOver<Webpage>().List();
                var publishOn = CurrentRequestData.Now;
                webpages.ForEach(webpage =>
                {
                    webpage.PublishOn = publishOn;
                    session.Update(webpage);
                });
            });
        }

        private ErrorPages GetErrorPages()
        {
            var error403 = new TextPage
            {
                Name = "403",
                UrlSegment = "403",
                BodyContent = "<h1>403</h1><p>Sorry, you are not authorized to view this page.</p>",
                RevealInNavigation = false,
            };
            var error404 = new TextPage
            {
                Name = "404",
                UrlSegment = "404",
                BodyContent = "<h1>404</h1><p>Sorry, this page cannot be found.</p>",
                RevealInNavigation = false,
            };
            var error500 = new TextPage
            {
                Name = "500",
                UrlSegment = "500",
                BodyContent = "<h1>500</h1><p>Sorry, there has been an error.</p>",
                RevealInNavigation = false,
            };
            return new ErrorPages
            {
                Error403 = error403,
                Error404 = error404,
                Error500 = error500
            };
        }
        private SearchPage GetSearchPage()
        {
            return new SearchPage
            {
                Name = "Search",
                UrlSegment = "search",
            };
        }

        private IEnumerable<Webpage> GetAccountPages()
        {
            var loginPage = new LoginPage
            {
                Name = "Login",
                UrlSegment = "login",
                DisplayOrder = 100,
                RevealInNavigation = false
            };

            yield return loginPage;
            var forgottenPassword = new ForgottenPasswordPage
            {
                Name = "Forgot Password",
                UrlSegment = "forgot-password",
                Parent = loginPage,
                DisplayOrder = 0,
                RevealInNavigation = false
            };
            yield return forgottenPassword;

            var resetPassword = new ResetPasswordPage
            {
                Name = "Reset Password",
                UrlSegment = "reset-password",
                Parent = loginPage,
                DisplayOrder = 1,
                RevealInNavigation = false
            };
            yield return resetPassword;

            var userAccountPage = new UserAccountPage
            {
                Name = "My Account",
                UrlSegment = "my-account",
                Parent = loginPage,
                DisplayOrder = 1,
                RevealInNavigation = false
            };
            yield return userAccountPage;

            var registerPage = new RegisterPage
            {
                Name = "Register",
                UrlSegment = "register",
            };
            yield return registerPage;
        }


        private IEnumerable<Webpage> GetBasicPages()
        {
            var homePage = new TextPage
            {
                Name = "Home",
                UrlSegment = "home",
                BodyContent =
                    "<h1>Mr CMS</h1> <p>Welcome to Mr CMS, the only CMS you will need.</p><p> Turn on inline editing above, then click here. Pretty cool huh? </p>",
                RevealInNavigation = true,
            };
            CurrentRequestData.HomePage = homePage;
            yield return homePage;
            yield return new TextPage
            {
                Name = "Page 2",
                UrlSegment = "page-2",
                BodyContent = "<h1>Another page</h1><p>Just another page!</p>",
                RevealInNavigation = true,
            };
            //contact us
            var contactUs = new TextPage
            {
                Name = "Contact us",
                UrlSegment = "contact-us",
                BodyContent = "<h1>Contact</h1>Contact us at www.mrcms.com (coming soon). <p>Test form</a> [form]",
                RevealInNavigation = true,
            };
            AddFormToContactUs(contactUs);
            yield return contactUs;
        }


        private void AddFormToContactUs(Webpage contactUs)
        {
            var fieldName = new TextBox
            {
                Name = "Name",
                LabelText = "Your Name",
                Required = true,
                Webpage = contactUs,
                DisplayOrder = 0
            };

            var fieldEmail = new TextBox
            {
                Name = "Email",
                LabelText = "Your Email",
                Required = true,
                Webpage = contactUs,
                DisplayOrder = 1
            };
            _session.Transact(s =>
            {
                s.Save(fieldName);
                s.Save(fieldEmail);
            });
        }
    }
}