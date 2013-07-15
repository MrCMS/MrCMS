using System;
using System.Collections.Generic;
using MrCMS.ACL;
using MrCMS.Entities.Widget;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class MrCMSWidgetACLAttribute : MrCMSBaseAuthorizationAttribute
    {
        private readonly string _operation;

        public MrCMSWidgetACLAttribute(string operation)
        {
            _operation = operation;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var routeData = httpContext.Request.RequestContext.RouteData;

            int id;
            if (int.TryParse(Convert.ToString(routeData.Values["id"]), out id))
            {
                try
                {
                    var widget = MrCMSApplication.Get<IWidgetService>().GetWidget<Widget>(id);

                    return CurrentRequestData.CurrentUser.CanAccess<WidgetACL>(_operation,
                                                                               new Dictionary<string, string>
                                                                                   {
                                                                                       {
                                                                                           "widget-type", widget.GetType().FullName
                                                                                       }
                                                                                   });
                }
                catch
                {
                    return false;
                }
            }
            else return false;
        }
    }
}