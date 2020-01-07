using MrCMS.Entities.People;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class DocumentRolesAdminService : IDocumentRolesAdminService
    {
        private readonly IGlobalRepository<UserRole> _repository;

        public DocumentRolesAdminService(IGlobalRepository<UserRole> repository)
        {
            _repository = repository;
        }

        public ISet<UserRole> GetFrontEndRoles(string frontEndRoles, bool inheritFromParent)
        {
            if (frontEndRoles == null)
            {
                frontEndRoles = string.Empty;
            }

            if (inheritFromParent)
            {
                return new HashSet<UserRole>();
            }

            var roleNames =
                frontEndRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            return roleNames.Select(GetRole).Where(x => x != null).ToHashSet();
        }

        private UserRole GetRole(string name)
        {
            return _repository.Query().SingleOrDefault(role => EF.Functions.Like(role.Name, name));
        }
    }
}