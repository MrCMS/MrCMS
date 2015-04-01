using System;
using MrCMS.ACL;
using NHibernate;

namespace MrCMS.Website
{
    public class MrCMSTypeACLAttribute : MrCMSBaseAuthorizationAttribute
    {
        private readonly Type _type;
        private readonly string _operation;

        public MrCMSTypeACLAttribute(Type type, string operation)
        {
            _type = type;
            _operation = operation;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (!CurrentRequestData.CurrentUser.IsActive)
                return false;
            object idVal;
            if (httpContext.Request.RequestContext.RouteData.Values.TryGetValue("id", out idVal))
            {
                int id;
                if (int.TryParse(Convert.ToString(idVal), out id))
                {
                    var o = MrCMSApplication.Get<ISession>().Get(_type, id);
                    if (o == null)
                        return false;

                    return new TypeACLRule().CanAccess(CurrentRequestData.CurrentUser, _operation, o.GetType().FullName);
                }
            }
            return false;
        }
    }
}