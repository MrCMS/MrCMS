using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreWebpages : ISetupCoreWebpages
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<FormProperty> _formPropertyRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public SetupCoreWebpages(IRepository<Webpage> repository,
            IRepository<Form> formRepository,
            IRepository<FormProperty> formPropertyRepository,
            IConfigurationProvider configurationProvider)
        {
            _repository = repository;
            _formRepository = formRepository;
            _formPropertyRepository = formPropertyRepository;
            _configurationProvider = configurationProvider;
        }

        private class ErrorPages
        {
            public Webpage Error403 { get; set; }
            public Webpage Error404 { get; set; }
            public Webpage Error500 { get; set; }
        }

        public async Task Setup()
        {
            ErrorPages errorPages = GetErrorPages();
            await _repository.Transact(async (repo, ct) =>
             {
                 await repo.AddRange(GetBasicPages().ToList(), ct);

                 await repo.Add(errorPages.Error403, ct);
                 await repo.Add(errorPages.Error404, ct);
                 await repo.Add(errorPages.Error500, ct);
             });

            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            siteSettings.Error403PageId = errorPages.Error403.Id;
            siteSettings.Error404PageId = errorPages.Error404.Id;
            siteSettings.Error500PageId = errorPages.Error500.Id;
            await _configurationProvider.SaveSettings(siteSettings);

            await _repository.Transact(async (repo, ct) =>
            {
                await repo.AddRange(GetAccountPages().ToList(), ct);

                await repo.Add(GetSearchPage(), ct);

                var webpages = repo.Query().ToList();
                var publishOn = DateTime.UtcNow;
                webpages.ForEach(webpage =>
                {
                    webpage.PublishOn = publishOn;
                });
                await repo.UpdateRange(webpages, ct);
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
            yield return new ForgottenPasswordPage
            {
                Name = "Forgot Password",
                UrlSegment = "forgot-password",
                Parent = loginPage,
                DisplayOrder = 0,
                RevealInNavigation = false
            };

            yield return new ResetPasswordPage
            {
                Name = "Reset Password",
                UrlSegment = "reset-password",
                Parent = loginPage,
                DisplayOrder = 1,
                RevealInNavigation = false
            };

            yield return new UserAccountPage
            {
                Name = "My Account",
                UrlSegment = "my-account",
                Parent = loginPage,
                DisplayOrder = 1,
                RevealInNavigation = false
            };
            yield return new TwoFactorCodePage
            {
                Name = "Verify Code",
                UrlSegment = "verify-code",
                BodyContent = "An email has been sent to your email address with your authentication code. Please enter this code below to authorise your login.",
                Parent = loginPage,
                DisplayOrder = 4,
                RevealInNavigation = false
            };

            yield return new RegisterPage
            {
                Name = "Register",
                UrlSegment = "register",
            };
        }


        private IEnumerable<Webpage> GetBasicPages()
        {
            var homePage = new TextPage
            {
                Name = "Home",
                UrlSegment = "home",
                BodyContent =
                    "<h1>Mr CMS</h1> <p>Welcome to Mr CMS.</p><p> Turn on inline editing above, then click here to start editing. </p>",
                RevealInNavigation = true,
            };
            //CurrentRequestData.HomePage = homePage;
            yield return homePage;
            yield return new TextPage
            {
                Name = "Page 2",
                UrlSegment = "page-2",
                BodyContent = "<h1>Another page</h1><p>Just another page!</p>",
                RevealInNavigation = true,
            };
            //contact us
            var form = AddContactUsForm();
            var contactUs = new TextPage
            {
                Name = "Contact us",
                UrlSegment = "contact-us",
                BodyContent = $"<h1>Contact</h1>Contact us at www.mrcms.com (coming soon). <p>Test form</a> [form id=\"{form.Id}\"]",
                RevealInNavigation = true,
            };
            yield return contactUs;
        }


        private async Task<Form> AddContactUsForm()
        {
            var form = new Form { Name = "Contact Us Form" };
            await _formRepository.Add(form);
            var fieldName = new TextBox
            {
                Name = "Name",
                LabelText = "Your Name",
                Required = true,
                FormId = form.Id,
                DisplayOrder = 0
            };

            var fieldEmail = new TextBox
            {
                Name = "Email",
                LabelText = "Your Email",
                Required = true,
                FormId = form.Id,
                DisplayOrder = 1
            };
            await _formPropertyRepository.AddRange(new List<TextBox>
            {
                fieldName,
                fieldEmail
            });
            return form;
        }
    }
}