using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Helpers;
using System.Linq;
using NHibernate;

namespace MrCMS.Entities.Documents.Web
{
    public abstract class Webpage : Document
    {
        private Layout.Layout _layout;
        private readonly AdminRoleUpdater _adminRoleUpdater;
        private readonly FrontEndRoleUpdater _frontEndRoleUpdater;

        protected Webpage()
        {
            _adminRoleUpdater = new AdminRoleUpdater(this);
            _frontEndRoleUpdater = new FrontEndRoleUpdater(this);
        }

        [DisplayName("Meta Title")]
        public virtual string MetaTitle { get; set; }
        [DisplayName("Meta Description")]
        public virtual string MetaDescription { get; set; }
        [DisplayName("Meta Keywords")]
        public virtual string MetaKeywords { get; set; }
        [DisplayName("Reveal in navigation")]
        public virtual bool RevealInNavigation { get; set; }

        [DisplayName("Requires SSL")]
        public virtual bool RequiresSSL { get; set; }

        public virtual bool Published
        {
            get { return PublishOn != null && PublishOn <= DateTime.UtcNow; }
        }

        public virtual string LiveUrlSegment
        {
            get { return MrCMSApplication.PublishedRootChildren().FirstOrDefault() == this ? string.Empty : UrlSegment; }
        }

        [UIHint("DateTime")]
        [DisplayName("Publish On")]
        public virtual DateTime? PublishOn { get; set; }

        public virtual Layout.Layout Layout { get; set; } //if we want to override the default layout

        public virtual Layout.Layout CurrentLayout
        {
            get { return _layout ?? (_layout = Layout ?? MrCMSApplication.Get<IDocumentService>().GetDefaultLayout(this)); }
        }

        public virtual IList<Widget.Widget> ShownWidgets { get; set; }
        public virtual IList<Widget.Widget> HiddenWidgets { get; set; }

        public virtual IList<Widget.Widget> Widgets { get; set; }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        [AllowHtml]
        [DisplayName("Body Content")]
        public virtual string BodyContent { get; set; }

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

        public virtual AdminRoleUpdater AdminRoleUpdater
        {
            get { return _adminRoleUpdater; }
        }

        public virtual FrontEndRoleUpdater FrontEndRoleUpdater
        {
            get { return _frontEndRoleUpdater; }
        }

        public virtual string AbsoluteUrl
        {
            get
            {
                var scheme = RequiresSSL ? "https://" : "http://";
                var authority = Site.BaseUrl;
                if (authority.EndsWith("/"))
                    authority = authority.TrimEnd('/');

                return string.Format("{0}{1}/{2}", scheme, authority, LiveUrlSegment);
            }
        }


        public virtual IEnumerable<RoleModel> GetFrontEndRoles()
        {
            return Roles.Provider.GetAllRoles().Select(roleName =>
                                                       new RoleModel
                                                           {
                                                               Name = roleName,
                                                               Status = GetRoleStatus(
                                                                   webpage => webpage.FrontEndAllowedRoles,
                                                                   webpage => webpage.FrontEndDisallowedRoles,
                                                                   (webpage, s) => webpage.IsAllowed(roleName),
                                                                   roleName),
                                                               Recursive = GetRoleIsRecursive(
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
                                                                   GetRoleStatus(
                                                                       webpage => webpage.AdminAllowedRoles,
                                                                       webpage => webpage.AdminDisallowedRoles,
                                                                       (webpage, s) =>
                                                                       webpage.IsAllowedForAdmin(roleName),
                                                                       roleName),
                                                               Recursive = GetRoleIsRecursive(
                                                                   webpage => webpage.AdminAllowedRoles,
                                                                   webpage => webpage.AdminDisallowedRoles, roleName)
                                                           });
        }

        private bool? GetRoleIsRecursive(Func<Webpage, IEnumerable<IRole>> allowedRoles, Func<Webpage, IEnumerable<IRole>> disallowedRoles, string roleName)
        {
            return allowedRoles(this).Any(role => role.UserRole.Name == roleName)
                       ? allowedRoles(this).First(role => role.UserRole.Name == roleName).IsRecursive
                       : (disallowedRoles(this).Any(role => role.UserRole.Name == roleName)
                              ? disallowedRoles(this).First(role => role.UserRole.Name == roleName).IsRecursive
                              : null);
        }

        private RoleStatus GetRoleStatus(Func<Webpage, IEnumerable<IRole>> allowedRoles, Func<Webpage, IEnumerable<IRole>> disallowedRoles,
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
            FrontEndRoleUpdater.UpdateFrontEndRoleStatuses(controllerContext, session);

            FrontEndRoleUpdater.UpdateFrontEndRoleRecursive(controllerContext, session);

            AdminRoleUpdater.UpdateAdminRoleStatuses(controllerContext, session);

            AdminRoleUpdater.UpdateAdminRoleRecursive(controllerContext, session);
        }

        public virtual void AdminViewData(ViewDataDictionary viewData, ISession session)
        {
        }

        public virtual void AddCustomSitemapData(UrlHelper urlHelper, XmlNode url, XmlDocument xmlDocument)
        {
        }

        public virtual void UiViewData(ViewDataDictionary viewData, ISession session, HttpRequestBase request)
        {
        }
    }
}