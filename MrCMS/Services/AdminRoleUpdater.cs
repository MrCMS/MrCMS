using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using NHibernate;

namespace MrCMS.Services
{
    public class AdminRoleUpdater
    {
        private readonly Webpage _webpage;

        public AdminRoleUpdater(Webpage webpage)
        {
            _webpage = webpage;
        }

        public void UpdateAdminRoleRecursive(ControllerContext controllerContext, ISession session)
        {
            foreach (
                string key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("Admin.Recursive")))
            {
                string[] parts = key.Split('.');
                string roleName = parts[1];
                UserRole role =
                    session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                string value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "True":
                        foreach (
                            AdminAllowedRole source in
                                _webpage.AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        foreach (
                            AdminDisallowedRole source in
                                _webpage.AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        break;
                    case "False":
                        foreach (
                            AdminAllowedRole source in
                                _webpage.AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        foreach (
                            AdminDisallowedRole source in
                                _webpage.AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        break;
                    case "":
                        foreach (
                            AdminAllowedRole source in
                                _webpage.AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        foreach (
                            AdminDisallowedRole source in
                                _webpage.AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        break;
                }
            }
        }

        public void UpdateAdminRoleStatuses(ControllerContext controllerContext, ISession session)
        {
            var roleService = new RoleService(session);
            foreach (
                string key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("Admin.Status")))
            {
                string[] parts = key.Split('.');
                string roleName = parts[1];
                UserRole role = roleService.GetRoleByName(roleName);

                string value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "Any":
                        {
                            AdminAllowedRole allowedRole =
                                _webpage.AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.AdminAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }
                            AdminDisallowedRole disallowedRole =
                                _webpage.AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.AdminDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                        }
                        break;
                    case "Allowed":
                        {
                            AdminDisallowedRole disallowedRole =
                                _webpage.AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.AdminDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                            AdminAllowedRole allowedRole =
                                _webpage.AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole == null)
                            {
                                var newRole = new AdminAllowedRole
                                                  {
                                                      Webpage = _webpage,
                                                      UserRole = role
                                                  };
                                _webpage.AdminAllowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                    case "Disallowed":
                        {
                            AdminAllowedRole allowedRole =
                                _webpage.AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.AdminAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }

                            AdminDisallowedRole disallowedRole =
                                _webpage.AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole == null)
                            {
                                var newRole = new AdminDisallowedRole
                                                  {
                                                      Webpage = _webpage,
                                                      UserRole = role
                                                  };
                                _webpage.AdminDisallowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                }
            }
        }
    }
}