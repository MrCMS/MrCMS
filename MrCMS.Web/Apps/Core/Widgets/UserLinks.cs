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
                navigationRecords.Add(new NavigationRecord
                                          {
                                              Text = MvcHtmlString.Create("My Account"),
                                              Url =
                                                  MvcHtmlString.Create("/" +
                                                                       documentService.GetUniquePage<UserAccountPage>()
                                                                                      .LiveUrlSegment)
                                          });
            }
            else
            {
                navigationRecords.Add(new NavigationRecord
                {
                    Text = MvcHtmlString.Create("Login"),
                    Url =
                        MvcHtmlString.Create("/" +
                                             documentService.GetUniquePage<LoginPage>()
                                                            .LiveUrlSegment)
                });
                navigationRecords.Add(new NavigationRecord
                {
                    Text = MvcHtmlString.Create("Register"),
                    Url =
                        MvcHtmlString.Create("/" +
                                             documentService.GetUniquePage<RegisterPage>()
                                                            .LiveUrlSegment)
                });
            }
            return navigationRecords;
        }
    }
}