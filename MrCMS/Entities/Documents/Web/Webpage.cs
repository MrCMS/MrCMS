using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Website;
using MrCMS.Helpers;
using System.Linq;
using NHibernate;

namespace MrCMS.Entities.Documents.Web
{
    public abstract class Webpage : Document
    {
        private Layout.Layout _layout;

        [DisplayName("Meta Title")]
        public virtual string MetaTitle { get; set; }
        [DisplayName("Meta Description")]
        public virtual string MetaDescription { get; set; }
        [DisplayName("Meta Keywords")]
        public virtual string MetaKeywords { get; set; }
        [DisplayName("Reveal in navigation")]
        public virtual bool RevealInNavigation { get; set; }

        public virtual bool Published
        {
            get { return PublishOn != null; }
        }

        [UIHint("DateTime")]
        [DisplayName("Publish On")]
        public virtual DateTime? PublishOn { get; set; }

        public virtual Layout.Layout Layout { get; set; } //if we want to override the default layout

        public virtual IList<LayoutAreaOverride> LayoutAreaOverrides { get; set; }

        public virtual Layout.Layout CurrentLayout
        {
            get { return _layout ?? (_layout = Layout ?? MrCMSApplication.GetDefaultLayout(this)); }
        }

        [DisplayName("Children inherit these layout options")]
        public virtual bool ChildrenInheritCustomLayoutOptions { get; set; }

        public virtual IList<Widget.Widget> ShownWidgets { get; set; }
        public virtual IList<Widget.Widget> HiddenWidgets { get; set; }

        public virtual IList<Widget.Widget> Widgets { get; set; }

        public virtual Webpage CustomLayoutParent
        {
            get
            {
                var parent = Parent.Unproxy() as Webpage;

                while (parent != null)
                {
                    if (parent.ChildrenInheritCustomLayoutOptions)
                        return parent;

                    parent = parent.Parent.Unproxy() as Webpage;
                }

                return null;
            }
        }

        public virtual Webpage RootPage
        {
            get
            {
                if (Parent == null)
                    return this;


                var parent = Parent.Unproxy() as Webpage;

                while (parent != null)
                {
                    if (parent.Parent.Unproxy() == null)
                        return parent;

                    parent = parent.Parent.Unproxy() as Webpage;
                }

                return null;
            }
        }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        public virtual IEnumerable<Webpage> ActivePages
        {
            get
            {
                var page = this;
                while (page != null)
                {
                    yield return page;
                    page = page.Parent.Unproxy() as Webpage;
                }
            }
        }

        public virtual bool IsHidden(Widget.Widget widget)
        {
            if (widget.Webpage == this)
                return false;

            foreach (var item in ActivePages)
            {
                if (item.HiddenWidgets.Contains(widget))
                    return true;
                if (item.ShownWidgets.Contains(widget))
                    return false;

                // if it's not overidden somehow and it is from the item we're looking at, use the recursive flag from the widget
                if (widget.Webpage.Unproxy() == item)
                    return !widget.IsRecursive;
            }
            return false;
        }

        public virtual bool IsAllowedForAdmin(User user)
        {
            var userRoles = user == null ? new List<UserRole>() : user.Roles;
            var anyRoles = false;
            foreach (var item in ActivePages)
            {
                if (item.AdminDisallowedRoles
                    .Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault())
                    .Any(role => userRoles.Contains(role.UserRole)))
                    return false;
                if (item.AdminAllowedRoles
                    .Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault())
                    .Any(role => userRoles.Contains(role.UserRole)))
                    return true;

                anyRoles = anyRoles || item.AdminDisallowedRoles.Any() || item.AdminAllowedRoles.Any();
            }
            return !AnyRoles(webpage => webpage.AdminAllowedRoles, webpage => webpage.AdminDisallowedRoles) && !BlockAnonymousAccess;
        }

        public virtual bool IsAllowed(User user)
        {
            var userRoles = user == null ? new List<UserRole>() : user.Roles;
            var anyRoles = false;
            foreach (var item in ActivePages)
            {
                if (item.FrontEndDisallowedRoles
                    .Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault())
                    .Any(role => userRoles.Contains(role.UserRole)))
                    return false;
                if (item.FrontEndAllowedRoles
                    .Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault())
                    .Any(role => userRoles.Contains(role.UserRole)))
                    return true;

                anyRoles = anyRoles || item.FrontEndDisallowedRoles.Any() || item.FrontEndAllowedRoles.Any();
            }
            return !AnyRoles(webpage => webpage.FrontEndAllowedRoles, webpage => webpage.FrontEndDisallowedRoles) && !BlockAnonymousAccess;
        }

        public virtual bool IsAllowed(string roleName)
        {
            var userRoles = Roles.GetAllRoles().Where(s => s == roleName).ToList();
            var anyRoles = false;
            foreach (var item in ActivePages)
            {
                if (item.FrontEndDisallowedRoles.Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault()).Any(role => userRoles.Contains(role.UserRole.Name)))
                    return false;
                if (item.FrontEndAllowedRoles.Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault()).Any(role => userRoles.Contains(role.UserRole.Name)))
                    return true;

                anyRoles = anyRoles || item.FrontEndDisallowedRoles.Any() || item.FrontEndAllowedRoles.Any();
            }
            return !AnyRoles(webpage => webpage.FrontEndAllowedRoles, webpage => webpage.FrontEndDisallowedRoles) || !BlockAnonymousAccess;
        }

        public virtual bool IsAllowedForAdmin(string roleName)
        {
            var userRoles = Roles.GetAllRoles().Where(s => s == roleName).ToList();
            var anyRoles = false;
            foreach (var item in ActivePages)
            {
                if (item.AdminDisallowedRoles.Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault()).Any(role => userRoles.Contains(role.UserRole.Name)))
                    return false;
                if (item.AdminAllowedRoles.Where(role => role.Webpage == this || role.IsRecursive.GetValueOrDefault()).Any(role => userRoles.Contains(role.UserRole.Name)))
                    return true;

                anyRoles = anyRoles || item.AdminDisallowedRoles.Any() || item.AdminAllowedRoles.Any();
            }
            return !AnyRoles(webpage => webpage.AdminAllowedRoles, webpage => webpage.AdminDisallowedRoles) || !BlockAnonymousAccess;
        }

        protected bool AnyRoles(Func<Webpage, IEnumerable<IRole>> getAllowedRoles, Func<Webpage, IEnumerable<IRole>> getDisallowedRoles)
        {
            return ActivePages.Aggregate(false,
                                         (current, item) =>
                                         current || getAllowedRoles(item).Any() || getDisallowedRoles(item).Any());
        }

        [DisplayName("Block Anonymous Access")]
        public virtual bool BlockAnonymousAccess { get; set; }

        [DisplayName("Form Data")]
        public virtual string FormData { get; set; }

        public virtual IList<FormPosting> FormPostings { get; set; }

        [DisplayName("Form Submitted Message")]
        public virtual string FormSubmittedMessage { get; set; }
        [DisplayName("Form Email Title")]
        public virtual string FormEmailTitle { get; set; }
        [DisplayName("Send Form To")]
        public virtual string SendFormTo { get; set; }
        [DisplayName("Form Email Message")]
        public virtual string FormMessage { get; set; }

        public virtual IList<FrontEndAllowedRole> FrontEndAllowedRoles { get; set; }
        public virtual IList<FrontEndDisallowedRole> FrontEndDisallowedRoles { get; set; }

        public virtual IList<AdminAllowedRole> AdminAllowedRoles { get; set; }
        public virtual IList<AdminDisallowedRole> AdminDisallowedRoles { get; set; }


        public virtual IEnumerable<RoleModel> GetFrontEndRoles()
        {
            return Roles.Provider.GetAllRoles().Select(roleName =>
                                                       new RoleModel
                                                           {
                                                               Name = roleName,
                                                               Status = GetStatus(
                                                                   webpage => webpage.FrontEndAllowedRoles,
                                                                   webpage => webpage.FrontEndDisallowedRoles,
                                                                   (webpage, s) => webpage.IsAllowed(roleName),
                                                                   roleName),
                                                               Recursive = GetRecursive(
                                                                   webpage => webpage.FrontEndAllowedRoles,
                                                                   webpage => webpage.FrontEndDisallowedRoles,
                                                                   roleName)
                                                           });
        }
        public virtual IEnumerable<RoleModel> GetAdminRoles()
        {
            return Roles.Provider.GetAllRoles().Select(roleName =>
                                                       new RoleModel
                                                           {
                                                               Name = roleName,
                                                               Status =
                                                                   GetStatus(
                                                                       webpage => webpage.AdminAllowedRoles,
                                                                       webpage => webpage.AdminDisallowedRoles,
                                                                       (webpage, s) =>
                                                                       webpage.IsAllowedForAdmin(roleName),
                                                                       roleName),
                                                               Recursive = GetRecursive(
                                                                   webpage => webpage.AdminAllowedRoles,
                                                                   webpage => webpage.AdminDisallowedRoles, roleName)
                                                           });
        }

        private bool? GetRecursive(Func<Webpage, IEnumerable<IRole>> allowedRoles, Func<Webpage, IEnumerable<IRole>> disallowedRoles, string roleName)
        {
            return allowedRoles(this).Any(role => role.UserRole.Name == roleName)
                       ? allowedRoles(this).First(role => role.UserRole.Name == roleName).IsRecursive
                       : (disallowedRoles(this).Any(role => role.UserRole.Name == roleName)
                              ? disallowedRoles(this).First(role => role.UserRole.Name == roleName).IsRecursive
                              : null);
        }

        private RoleStatus GetStatus(Func<Webpage, IEnumerable<IRole>> allowedRoles, Func<Webpage, IEnumerable<IRole>> disallowedRoles,
            Func<Webpage, string, bool> func, string roleName)
        {
            return !AnyRoles(allowedRoles, disallowedRoles)
                       ? RoleStatus.Any
                       : (func(this, roleName)
                              ? RoleStatus.Allowed
                              : RoleStatus.Disallowed);
        }

        public override void CustomBinding(ControllerContext controllerContext, ISession session)
        {
            foreach (var key in controllerContext.HttpContext.Request.Form.Keys.Cast<string>().Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Status")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "Any":
                        {
                            var allowedRole = FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }
                            var disallowedRole = FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                        }
                        break;
                    case "Allowed":
                        {
                            var disallowedRole = FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                FrontEndDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                            var allowedRole = FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole == null)
                            {
                                var newRole = new FrontEndAllowedRole
                                                  {
                                                      Webpage = this,
                                                      UserRole = role
                                                  };
                                FrontEndAllowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                    case "Disallowed":
                        {
                            var allowedRole = FrontEndAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                FrontEndAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }

                            var disallowedRole = FrontEndDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole == null)
                            {
                                var newRole = new FrontEndDisallowedRole
                                                  {
                                                      Webpage = this,
                                                      UserRole = role
                                                  };
                                FrontEndDisallowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                }
            }


            foreach (var key in controllerContext.HttpContext.Request.Form.Keys.Cast<string>().Where(s => s.StartsWith("role.") && s.EndsWith("FrontEnd.Recursive")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "True":
                        foreach (var source in FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        foreach (var source in FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        break;
                    case "False":
                        foreach (var source in FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        foreach (var source in FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        break;
                    case "":
                        foreach (var source in FrontEndAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        foreach (var source in FrontEndDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        break;
                }
            }

            foreach (var key in controllerContext.HttpContext.Request.Form.Keys.Cast<string>().Where(s => s.StartsWith("role.") && s.EndsWith("Admin.Status")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "Any":
                        {
                            var allowedRole = AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                AdminAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }
                            var disallowedRole = AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                AdminDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                        }
                        break;
                    case "Allowed":
                        {
                            var disallowedRole = AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole != null)
                            {
                                AdminDisallowedRoles.Remove(disallowedRole);
                                session.Delete(disallowedRole);
                            }
                            var allowedRole = AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole == null)
                            {
                                var newRole = new AdminAllowedRole
                                {
                                    Webpage = this,
                                    UserRole = role
                                };
                                AdminAllowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                    case "Disallowed":
                        {
                            var allowedRole = AdminAllowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (allowedRole != null)
                            {
                                AdminAllowedRoles.Remove(allowedRole);
                                session.Delete(allowedRole);
                            }

                            var disallowedRole = AdminDisallowedRoles.FirstOrDefault(role1 => role1.UserRole.Name == roleName);
                            if (disallowedRole == null)
                            {
                                var newRole = new AdminDisallowedRole
                                {
                                    Webpage = this,
                                    UserRole = role
                                };
                                AdminDisallowedRoles.Add(newRole);
                                session.SaveOrUpdate(newRole);
                            }
                        }
                        break;
                }
            }


            foreach (var key in controllerContext.HttpContext.Request.Form.Keys.Cast<string>().Where(s => s.StartsWith("role.") && s.EndsWith("Admin.Recursive")))
            {
                var parts = key.Split('.');
                var roleName = parts[1];
                var role = session.QueryOver<UserRole>().Where(userRole => userRole.Name == roleName).SingleOrDefault();

                var value = controllerContext.HttpContext.Request[key];

                switch (value)
                {
                    case "True":
                        foreach (var source in AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        foreach (var source in AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = true;
                        break;
                    case "False":
                        foreach (var source in AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        foreach (var source in AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = false;
                        break;
                    case "":
                        foreach (var source in AdminAllowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        foreach (var source in AdminDisallowedRoles.Where(allowedRole => allowedRole.UserRole == role))
                            source.IsRecursive = null;
                        break;
                }
            }
        }

        public virtual void AddTypeSpecificViewData(ViewDataDictionary viewData)
        {
        }
    }
}