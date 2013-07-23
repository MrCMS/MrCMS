using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Entities.People;
using System.Linq;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.ACL
{
    public abstract class ACLRule
    {
        private List<string> _operations;
        public string Name { get { return GetType().FullName; } }

        protected virtual MrCMSApp App { get { return null; } }
        public string AppName { get { return App == null ? "System" : App.AppName; } }

        public abstract string DisplayName { get; }

        public bool CanAccess(User user, string operation, string typeName = null)
        {
            return user.IsAdmin || user.Roles.Any(role => CanAccessLogic(role, operation, typeName));
        }

        public bool CanAccess(UserRole userRole, string operation, string typeName = null)
        {
            return userRole.IsAdmin || CanAccessLogic(userRole, operation, typeName);
        }

        private bool CanAccessLogic(UserRole userRole, string operation, string typeName = null)
        {
            if (!MrCMSApplication.Get<ACLSettings>().ACLEnabled)
                return false;
            var aclRoles = userRole.ACLRoles;
            var b = GetKey(operation, typeName);
            return aclRoles.Any(role => role.Name == b);
        }

        protected string GetKey(string operation, string typeName = null)
        {
            var start = !string.IsNullOrWhiteSpace(typeName)
                            ? string.Format("{0}.{1}", Name, typeName)
                            : Name;
            return !string.IsNullOrWhiteSpace(operation)
                        ? string.Format("{0}.{1}", start, operation)
                        : start;
        }

        public List<string> Operations
        {
            get { return _operations ?? (_operations = GetOperations()); }
        }

        protected abstract List<string> GetOperations();

        public virtual List<ACLGroup> GetRules()
        {
            return new List<ACLGroup>
                       {
                           new ACLGroup
                               {
                                   Name = DisplayName,
                                   AppName = AppName,
                                   Operations = Operations.Select(s => new ACLOperation
                                                                           {
                                                                               Name = s,
                                                                               Key = GetKey(s),
                                                                           }).ToList(),
                                   Type = null
                               }
                       };
        }
    }

    public abstract class ACLRule<T> : ACLRule where T : MrCMSApp, new()
    {
        protected override MrCMSApp App { get { return new T(); } }
    }
}