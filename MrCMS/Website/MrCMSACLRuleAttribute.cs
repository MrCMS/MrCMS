using System;
using MrCMS.ACL;

namespace MrCMS.Website
{
    public class MrCMSACLRuleAttribute : MrCMSBaseAuthorizationAttribute
    {
        private readonly Type _type;
        private readonly string _operation;

        public MrCMSACLRuleAttribute(Type type, string operation)
        {
            _type = type;
            _operation = operation;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (!CurrentRequestData.CurrentUser.IsActive)
                return false;
            var aclRule = (Activator.CreateInstance(_type) as ACLRule);
            return aclRule != null && aclRule.CanAccess(CurrentRequestData.CurrentUser, _operation, null);
        }
    }
}