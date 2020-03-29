using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class DocumentRolesAdminService : IDocumentRolesAdminService
    {
        private readonly IGlobalRepository<Role> _repository;

        public DocumentRolesAdminService(IGlobalRepository<Role> repository)
        {
            _repository = repository;
        }

        public IList<FrontEndAllowedRole> GetFrontEndRoles(Webpage webpage, string frontEndRoles,
            bool inheritFromParent)
        {
            if (frontEndRoles == null)
            {
                frontEndRoles = string.Empty;
            }

            if (inheritFromParent)
            {
                return new List<FrontEndAllowedRole>();
            }

            var roleNames =
                frontEndRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            return roleNames.Select(GetRole).Where(x => x != null)
                .Select(userRole => new FrontEndAllowedRole {Webpage = webpage, Role = userRole}).ToList();
        }

        private Role GetRole(string name)
        {
            return _repository.Query().SingleOrDefault(role => EF.Functions.Like(role.Name, name));
        }
    }
}