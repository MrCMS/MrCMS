using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using NHibernate;

namespace MrCMS.Services
{
    public class FrontEndRoleUpdater
    {
        private readonly Webpage _webpage;

        public FrontEndRoleUpdater(Webpage webpage)
        {
            _webpage = webpage;
        }

        public void UpdateFrontEndRoleRecursive(ControllerContext controllerContext, ISession session)
        {
            foreach (
                string key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Recursive")))
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
                            FrontEndAllowedRole source in
                                _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        foreach (
                            FrontEndDisallowedRole source in
                                _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        break;
                    case "False":
                        foreach (
                            FrontEndAllowedRole source in
                                _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        foreach (
                            FrontEndDisallowedRole source in
                                _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        break;
                    case "":
                        foreach (
                            FrontEndAllowedRole source in
                                _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        foreach (
                            FrontEndDisallowedRole source in
                                _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        break;
                }
            }
        }

        public void UpdateFrontEndRoleStatuses(ControllerContext controllerContext, ISession session)
        {
            foreach (
                string key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Status")))
            {
                string[] parts = key.Split('.');
                string roleName = parts[1];
                UserRole role =
                    session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                string value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "Any":
                        {
                            FrontEndAllowedRole allowedRole =
                                _webpage.FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }
                            FrontEndDisallowedRole disallowedRole =
                                _webpage.FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                        }
                        break;
                    case "Allowed":
                        {
                            FrontEndDisallowedRole disallowedRole =
                                _webpage.FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                            FrontEndAllowedRole allowedRole =
                                _webpage.FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole == null)
                            {
                                var newRole = new FrontEndAllowedRole
                                                  {
                                                      Webpage = _webpage,
                                                      UserRole = role
                                                  };
                                _webpage.FrontEndAllowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                    case "Disallowed":
                        {
                            FrontEndAllowedRole allowedRole =
                                _webpage.FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }

                            FrontEndDisallowedRole disallowedRole =
                                _webpage.FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole == null)
                            {
                                var newRole = new FrontEndDisallowedRole
                                                  {
                                                      Webpage = _webpage,
                                                      UserRole = role
                                                  };
                                _webpage.FrontEndDisallowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                }
            }
        }
    }
}