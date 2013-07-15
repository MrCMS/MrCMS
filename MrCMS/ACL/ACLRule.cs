using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Entities.People;
using System.Linq;

namespace MrCMS.ACL
{
    public abstract class ACLRule
    {
        private List<string> _operations;
        public string Name { get { return GetType().FullName; } }

        protected virtual MrCMSApp App { get { return null; } }
        public string AppName { get { return App == null ? "System" : App.AppName; } }

        public abstract string DisplayName { get; }

        public bool CanAccess(User user, string operation, IDictionary<string, string> customData)
        {
            return user.IsAdmin || user.Roles.Any(role => CanAccessLogic(role, operation, customData));
        }
        public bool CanAccess(UserRole userRole, string operation, IDictionary<string, string> customData)
        {
            return userRole.IsAdmin || CanAccessLogic(userRole, operation, customData);
        }

        private bool CanAccessLogic(UserRole userRole, string operation, IDictionary<string, string> customData)
        {
            var aclRoles = userRole.ACLRoles;
            var b = GetKey(operation, customData);
            return aclRoles.Any(role => role.Name == b);
        }

        public virtual string GetKey(string operation, IDictionary<string, string> customData)
        {
            var b = !string.IsNullOrWhiteSpace(operation)
                        ? string.Format("{0}.{1}", Name, operation)
                        : string.Format("{0}", Name);
            return b;
        }

        public List<string> Operations
        {
            get { return _operations ?? (_operations = GetOperations()); }
        }

        protected abstract List<string> GetOperations();

        public virtual IDictionary<string, List<ACLOperation>> GetRules()
        {
            return new Dictionary<string, List<ACLOperation>>
                       {
                           {
                               DisplayName, Operations.Select(s => new ACLOperation
                                                                       {
                                                                           Name = s,
                                                                           OperationKey =
                                                                               GetKey(s,
                                                                                      new Dictionary<string, string>())
                                                                       }).ToList()
                           }
                       };
        }
    }

    public class ACLOperation
    {
        public string Name { get; set; }

        public string OperationKey { get; set; }
    }

    public abstract class ACLRule<T> : ACLRule where T : MrCMSApp, new()
    {
        protected override MrCMSApp App { get { return new T(); } }
    }
}