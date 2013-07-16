using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Entities.ACL;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;

namespace MrCMS.Services
{
    public class ACLService : IACLService
    {
        private readonly IRoleService _roleService;
        private readonly ISession _session;

        public ACLService(IRoleService roleService, ISession session)
        {
            _roleService = roleService;
            _session = session;
        }

        public ACLModel GetACLModel()
        {
            return ACLModel.Create(_roleService, this);
        }

        public List<ACLRule> GetAllSystemRules()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<ACLRule>()
                             .Select(Activator.CreateInstance)
                             .Cast<ACLRule>().Where(rule => !(rule is TypeACLRule)).ToList();
        }

        public void UpdateACL(List<ACLUpdateRecord> model)
        {
            _session.Transact(session =>
            {
                foreach (var aclUpdateRecord in model)
                {
                    var role = _roleService.GetRoleByName(aclUpdateRecord.Role);

                    var aclRole = role.ACLRoles.FirstOrDefault(ar => ar.Name == aclUpdateRecord.Key);
                    if ((aclRole != null && aclUpdateRecord.Allowed) || (aclRole == null && !aclUpdateRecord.Allowed))
                        continue;
                    if (aclRole != null && !aclUpdateRecord.Allowed)
                    {
                        role.ACLRoles.Remove(aclRole);
                        session.Delete(aclRole);
                    }
                    else if (aclRole == null && aclUpdateRecord.Allowed)
                    {
                        var newRole = new ACLRole { UserRole = role, Name = aclUpdateRecord.Key };
                        role.ACLRoles.Add(newRole);
                        session.Save(newRole);
                    }
                }
            });
        }
    }
}