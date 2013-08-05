using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class UserLinks : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var navigationRecords = new List<NavigationRecord>();
            var documentService = MrCMSApplication.Get<IDocumentService>();

            var loggedIn = CurrentRequestData.CurrentUser != null;
            if (loggedIn)
            {
                var liveUrlSegment = documentService.GetUniquePage<UserAccountPage>().LiveUrlSegment;
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
                var liveUrlSegment = documentService.GetUniquePage<LoginPage>().LiveUrlSegment;
                if (liveUrlSegment != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create("Login"),
                        Url =
                            MvcHtmlString.Create(string.Format("/{0}", liveUrlSegment))
                    });
                    var urlSegment = documentService.GetUniquePage<RegisterPage>().LiveUrlSegment;
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