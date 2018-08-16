using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class DocumentRolesAdminService : IDocumentRolesAdminService
    {
        private readonly ISession _session;

        public DocumentRolesAdminService(ISession session)
        {
            _session = session;
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
            return _session.QueryOver<UserRole>().Where(role => role.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }
    }
}