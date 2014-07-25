using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
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

        public GetUserLinks(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        public override object GetModel(UserLinks widget)
        {
            var navigationRecords = new List<NavigationRecord>();

            var loggedIn = CurrentRequestData.CurrentUser != null;
            if (loggedIn)
            {
                var liveUrlSegment = _uniquePageService.GetUniquePage<UserAccountPage>().LiveUrlSegment;
                if (liveUrlSegment != null)
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create("My Account"),
                        Url =
                            MvcHtmlString.Create(string.Format("/{0}", liveUrlSegment))
                    });


                navigationRecords.Add(new NavigationRecord
                {
                    Text = MvcHtmlString.Create("Logout"),
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
                        Text = MvcHtmlString.Create("Login"),
                        Url =
                            MvcHtmlString.Create(string.Format("/{0}", liveUrlSegment))
                    });
                    var urlSegment = _uniquePageService.GetUniquePage<RegisterPage>().LiveUrlSegment;
                    if (urlSegment != null)
                        navigationRecords.Add(new NavigationRecord
                        {
                            Text = MvcHtmlString.Create("Register"),
                            Url =
                                MvcHtmlString.Create(string.Format("/{0}", urlSegment))
                        });
                }
            }
            return navigationRecords;
        }
    }
}