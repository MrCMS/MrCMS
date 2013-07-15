using System;
using System.Collections.Generic;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class MrCMSWebpageACLAttribute : MrCMSBaseAuthorizationAttribute
    {
        private readonly string _operation;

        public MrCMSWebpageACLAttribute(string operation)
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
                    var document = MrCMSApplication.Get<IDocumentService>().GetDocument<Document>(id);

                    return CurrentRequestData.CurrentUser.CanAccess<WebpageACL>(_operation,
                                                                                new Dictionary<string, string>
                                                                                    {
                                                                                        {
                                                                                            "page-type", document.GetType().FullName
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