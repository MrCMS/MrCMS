using System;
using System.Collections.Generic;
using MrCMS.ACL;
using MrCMS.ACL.Rules;

namespace MrCMS.Website
{
    public class MrCMSAuthorizeAttribute : MrCMSBaseAuthorizationAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var currentUser = CurrentRequestData.CurrentUser;
            return base.AuthorizeCore(httpContext) && currentUser != null &&
                   currentUser.IsActive &&
                   currentUser.Email == httpContext.User.Identity.Name &&
                   currentUser.CanAccess<AdminAccessACL>("Allowed");
        }
    }
    public class MrCMSACLAttribute : MrCMSBaseAuthorizationAttribute
    {
        private readonly Type _type;
        private readonly string _operation;

        public MrCMSACLAttribute(Type type, string operation)
        {
            _type = type;
            _operation = operation;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var aclRule = (Activator.CreateInstance(_type) as ACLRule);
            return aclRule.CanAccess(CurrentRequestData.CurrentUser, _operation, null);
        }
    }
}