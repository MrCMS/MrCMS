using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using MrCMS.Helpers;

namespace MrCMS.Installation
{
    internal class PermissionsChecker
    {
        private readonly WindowsIdentity _current;
        private readonly RequiredCheck _requiredCheck;
        private readonly HashSet<FileSystemAccessRule> _rules;
        private readonly HashSet<IdentityReference> _groups;

        public PermissionsChecker(WindowsIdentity current, AuthorizationRuleCollection rules,
            RequiredCheck requiredCheck)
        {
            _current = current;
            _groups = _current.Groups.ToHashSet();
            _rules =
                rules.Cast<FileSystemAccessRule>()
                    .Where(rule => _current.User.Equals(rule.IdentityReference))
                    .ToHashSet();
            _requiredCheck = requiredCheck;
        }

        public IEnumerable<string> InvalidReasons
        {
            get
            {
                if (_requiredCheck.CheckWrite && (!CanWrite || DeniedWrite))
                    yield return "Unable to write as required";
                if (_requiredCheck.CheckRead && (!CanRead || DeniedRead))
                    yield return "Unable to read as required";
                if (_requiredCheck.CheckModify && (!CanModify || DeniedModify))
                    yield return "Unable to modify as required";
                if (_requiredCheck.CheckDelete && (!CanDelete || DeniedDelete))
                    yield return "Unable to delete as required";
            }
        }

        public bool CanDelete
        {
            get { return Check(FileSystemRights.Delete, AccessControlType.Allow); }
        }

        public bool CanWrite
        {
            get { return Check(FileSystemRights.Write, AccessControlType.Allow); }
        }

        public bool CanModify
        {
            get { return Check(FileSystemRights.Modify, AccessControlType.Allow); }
        }

        public bool CanRead
        {
            get { return Check(FileSystemRights.Read, AccessControlType.Allow); }
        }

        public bool DeniedDelete
        {
            get { return Check(FileSystemRights.Delete, AccessControlType.Deny); }
        }

        public bool DeniedWrite
        {
            get { return Check(FileSystemRights.Write, AccessControlType.Deny); }
        }

        public bool DeniedModify
        {
            get { return Check(FileSystemRights.Modify, AccessControlType.Deny); }
        }

        public bool DeniedRead
        {
            get { return Check(FileSystemRights.Read, AccessControlType.Deny); }
        }

        public bool IsValid()
        {
            return !InvalidReasons.Any();
        }

        private bool CheckGroups(FileSystemRights rights, AccessControlType controlType)
        {
            return _groups.Any(reference =>
                _rules.Where(rule2 => reference.Equals(rule2.IdentityReference) &&
                                      controlType.Equals(rule2.AccessControlType))
                    .Any(rule2 => (rights & rule2.FileSystemRights) == rights));
        }

        private bool CheckUser(FileSystemRights rights, AccessControlType controlType)
        {
            return _rules.Where(rule => controlType.Equals(rule.AccessControlType))
                .Any(rule => (rights & rule.FileSystemRights) == rights);
        }

        private bool Check(FileSystemRights rights, AccessControlType controlType)
        {
            try
            {
                return CheckUser(rights, controlType) || CheckGroups(rights, controlType);
            }
            catch (IOException)
            {
            }
            return false;
        }
    }
}