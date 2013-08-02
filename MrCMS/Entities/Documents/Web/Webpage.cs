using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Helpers;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Entities.Documents.Web
{
    public abstract class Webpage : Document
    {
        protected Webpage()
        {
            InheritAdminRolesFromParent = true;
            InheritFrontEndRolesFromParent = true;
            Urls = new List<UrlHistory>();
        }
        private Layout.Layout _layout;

        [Required]
        [Remote("ValidateUrlIsAllowed", "Webpage", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Url Segment")]
        public override string UrlSegment { get; set; }

        [DisplayName("Meta Title")]
        [StringLength(250, ErrorMessage = "Meta title cannot be longer than 250 characters.")]
        public virtual string MetaTitle { get; set; }
        [DisplayName("Meta Description")]
        [StringLength(250, ErrorMessage = "Meta description cannot be longer than 250 characters.")]
        public virtual string MetaDescription { get; set; }
        [DisplayName("Meta Keywords")]
        [StringLength(250, ErrorMessage = "Meta keywords cannot be longer than 250 characters.")]
        public virtual string MetaKeywords { get; set; }
        [DisplayName("Include in navigation")]
        public virtual bool RevealInNavigation { get; set; }
        [DisplayName("Custom header scripts")]
        [StringLength(8000)]
        public virtual string CustomHeaderScripts { get; set; }
        [DisplayName("Custom footer scripts")]
        [StringLength(8000)]
        public virtual string CustomFooterScripts { get; set; }


        [DisplayName("Requires SSL")]
        public virtual bool RequiresSSL { get; set; }

        public virtual bool Published
        {
            get { return PublishOn != null && PublishOn <= CurrentRequestData.Now; }
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

        [DisplayName("Block Anonymous Access")]
        public virtual bool BlockAnonymousAccess { get; set; }
        
        //forms
        [DisplayName("Form Submitted Message")]
        [AllowHtml]
        [StringLength(500, ErrorMessage = "Form submitted messsage cannot be longer than 500 characters.")]
        public virtual string FormSubmittedMessage { get; set; }
        [DisplayName("Subject")]
        [StringLength(250, ErrorMessage = "Subject cannot be longer than 250 characters.")]
        public virtual string FormEmailTitle { get; set; }
        [DisplayName("Send Form To")]
        [StringLength(500, ErrorMessage = "Send to cannot be longer than 500 characters.")]
        public virtual string SendFormTo { get; set; }
        [DisplayName("Form Email Message")]
        public virtual string FormMessage { get; set; }
        public virtual IList<FormProperty> FormProperties { get; set; }
        public virtual IList<FormPosting> FormPostings { get; set; }
        public virtual string FormDesign { get; set; }
        [StringLength(100)]
        [DisplayName("Submit Button Css Class")]
        public virtual string SubmitButtonCssClass { get; set; }
        [StringLength(100)]
        [DisplayName("Submit button custom CSS class")]
        public virtual string SubmitButtonText { get; set; }
        

        [DisplayName("Same as parent")]
        public virtual bool InheritFrontEndRolesFromParent { get; set; }
        public virtual IList<UserRole> FrontEndAllowedRoles { get; set; }
        [DisplayName("Same as parent")]
        public virtual bool InheritAdminRolesFromParent { get; set; }
        public virtual IList<UserRole> AdminAllowedRoles { get; set; }

        [DisplayName("Roles")]
        public virtual string FrontEndRoles
        {
            get { return string.Join(", ", FrontEndAllowedRoles.Select(x => x.Name)); }
        }

        [DisplayName("Roles")]
        public virtual string AdminRoles
        {
            get { return string.Join(", ", AdminAllowedRoles.Select(x => x.Name)); }
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
        
        public virtual IList<UrlHistory> Urls { get; set; }

        public virtual IEnumerable<Webpage> PublishedChildren
        {
            get
            {
                return
                    Children.Select(webpage => webpage.Unproxy()).OfType<Webpage>().Where(document => document.Published)
                            .OrderBy(webpage => webpage.DisplayOrder);
            }
        }

        public virtual bool IsAllowed(User currentUser)
        {
            if (currentUser != null && currentUser.IsAdmin) return true;
            if (InheritFrontEndRolesFromParent)
            {
                if (Parent is Webpage)
                    return (Parent as Webpage).IsAllowed(currentUser);
                return true;
            }
            if (!FrontEndAllowedRoles.Any()) return true;
            if (FrontEndAllowedRoles.Any() && currentUser == null) return false;
            return currentUser != null && currentUser.Roles.Intersect(FrontEndAllowedRoles).Any();
        }

        public virtual bool IsAllowedForAdmin(User currentUser)
        {
            if (currentUser != null && currentUser.IsAdmin) return true;
            if (InheritAdminRolesFromParent)
            {
                if (Parent is Webpage)
                    return (Parent as Webpage).IsAllowedForAdmin(currentUser);
                return true;
            }
            if (!AdminAllowedRoles.Any()) return true;
            if (AdminAllowedRoles.Any() && currentUser == null) return false;
            return currentUser != null && currentUser.Roles.Intersect(AdminAllowedRoles).Any();
        }

        /// <summary>
        /// Method to page child items with default filter and ordering implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPagedList<T> PagedChildren<T>(QueryOver<T> query = null, int pageNum = 1, int pageSize = 10) where T : Webpage
        {
            query = query ??
                    QueryOver.Of<T>()
                             .Where(a => a.Parent == this && a.PublishOn != null && a.PublishOn <= CurrentRequestData.Now)
                             .ThenBy(arg => arg.PublishOn)
                             .Desc;

            return MrCMSApplication.Get<ISession>().Paged(query, pageNum, pageSize);
        }

        public virtual string GetPageTitle()
        {
            return !string.IsNullOrWhiteSpace(MetaTitle) ? MetaTitle : Name;
        }

        public virtual bool CanAddChildren()
        {
            return this.GetMetadata().ValidChildrenTypes.Any();
        }
        
        public override void CustomBinding(ControllerContext controllerContext, ISession session)
        {
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