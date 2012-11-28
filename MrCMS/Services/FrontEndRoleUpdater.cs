using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using NHibernate;

namespace MrCMS.Services
{
    public class FrontEndRoleUpdater
    {
        private Webpage _webpage;

        public FrontEndRoleUpdater(Webpage webpage)
        {
            _webpage = webpage;
        }

        public void UpdateFrontEndRoleRecursive(ControllerContext controllerContext, ISession session)
        {
            foreach (
                var key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Recursive")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "True":
                        foreach (var source in _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        foreach (var source in _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        break;
                    case "False":
                        foreach (var source in _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        foreach (var source in _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        break;
                    case "":
                        foreach (var source in _webpage.FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        foreach (var source in _webpage.FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        break;
                }
            }
        }

        public void UpdateFrontEndRoleStatuses(ControllerContext controllerContext, ISession session)
        {
            foreach (
                var key in
                    controllerContext.HttpContext.Request.Form.Keys.Cast<string>()
                                     .Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Status")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "Any":
                        {
                            var allowedRole = Enumerable.FirstOrDefault<FrontEndAllowedRole>(_webpage.FrontEndAllowedRoles, role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }
                            var disallowedRole = Enumerable.FirstOrDefault<FrontEndDisallowedRole>(_webpage.FrontEndDisallowedRoles, role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                        }
                        break;
                    case "Allowed":
                        {
                            var disallowedRole = Enumerable.FirstOrDefault<FrontEndDisallowedRole>(_webpage.FrontEndDisallowedRoles, role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                _webpage.FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                            var allowedRole = Enumerable.FirstOrDefault<FrontEndAllowedRole>(_webpage.FrontEndAllowedRoles, role1 => role1.UserRole.Name == roleName);
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
                            var allowedRole = Enumerable.FirstOrDefault<FrontEndAllowedRole>(_webpage.FrontEndAllowedRoles, role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                _webpage.FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }

                            var disallowedRole = Enumerable.FirstOrDefault<FrontEndDisallowedRole>(_webpage.FrontEndDisallowedRoles, role1 => role1.UserRole.Name == roleName);
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