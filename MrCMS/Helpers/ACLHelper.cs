using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class ACLHelper
    {
        public static bool CanAccess<T>(this HtmlHelper html, string operation, string type = null) where T : ACLRule, new()
        {
            return CurrentRequestData.CurrentUser != null &&
                   new T().CanAccess(CurrentRequestData.CurrentUser, operation, type);
        }
    }
}