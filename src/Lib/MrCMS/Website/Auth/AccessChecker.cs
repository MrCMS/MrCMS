using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public class AccessChecker : IAccessChecker
    {
        private readonly ICheckStandardAccessLogic _checkStandardAccessLogic;
        private readonly IGetAclKeys _getAclKeys;
        private readonly IPerformAclCheck _performAclCheck;

        public AccessChecker(ICheckStandardAccessLogic checkStandardAccessLogic,
            IGetAclKeys getAclKeys,
            IPerformAclCheck performAclCheck)
        {
            _checkStandardAccessLogic = checkStandardAccessLogic;
            _getAclKeys = getAclKeys;
            _performAclCheck = performAclCheck;
        }

        public bool CanAccess(ControllerActionDescriptor descriptor)
        {
            var result = _checkStandardAccessLogic.Check();
            var keys = _getAclKeys.GetKeys(descriptor);
            return _performAclCheck.CanAccessLogic(result, keys);
        }

        public bool CanAccess(ControllerActionDescriptor descriptor, User user)
        {
            var result = _checkStandardAccessLogic.Check(user);
            var keys = _getAclKeys.GetKeys(descriptor);
            return _performAclCheck.CanAccessLogic(result, keys);
        }


        public bool CanAccess<TAclRule>(string operation) where TAclRule : ACLRule
        {
            var result = _checkStandardAccessLogic.Check();
            var keys = _getAclKeys.GetKeys<TAclRule>(operation);
            return _performAclCheck.CanAccessLogic(result, keys);
        }

        public bool CanAccess<TAclRule>(string operation, User user) where TAclRule : ACLRule
        {
            var result = _checkStandardAccessLogic.Check(user);
            var keys = _getAclKeys.GetKeys<TAclRule>(operation);
            return _performAclCheck.CanAccessLogic(result, keys);
        }
    }
}