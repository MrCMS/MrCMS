using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetUserLinks:GetWidgetModelBase<UserLinks>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetUserLinks(IUniquePageService uniquePageService, IStringResourceProvider stringResourceProvider)
        {
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
        }

        public override object GetModel(UserLinks widget)
        {
            var navigationRecords = new List<NavigationRecord>();

            var loggedIn = CurrentRequestData.CurrentUser != null;
            if (loggedIn)
            {
                var userAccountPage = _uniquePageService.GetUniquePage<UserAccountPage>();
                if (userAccountPage != null)
                {
                    var liveUrlSegment = userAccountPage.LiveUrlSegment;
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("My Account")),
                        Url =
                            MvcHtmlString.Create(string.Format("/{0}", liveUrlSegment))
                    });
                }


                navigationRecords.Add(new NavigationRecord
                {
                    Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Logout")),
                    Url =
                        MvcHtmlString.Create(string.Format("/logout"))
                });
            }
            else
            {
                var liveUrlSegment = _uniquePageService.GetUniquePage<LoginPage>().LiveUrlSegment;
                if (liveUrlSegment != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Login")),
                        Url =
                            MvcHtmlString.Create(string.Format("/{0}", liveUrlSegment))
                    });
                    var urlSegment = _uniquePageService.GetUniquePage<RegisterPage>().LiveUrlSegment;
                    if (urlSegment != null)
                        navigationRecords.Add(new NavigationRecord
                        {
                            Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Register")),
                            Url =
                                MvcHtmlString.Create(string.Format("/{0}", urlSegment))
                        });
                }
            }
            return navigationRecords;
        }
    }
}