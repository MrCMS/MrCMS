using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Entities.Documents.Web
{
    public abstract class Webpage : Document
    {
        public enum WebpagePublishStatus
        {
            Published,
            Scheduled,
            Unpublished
        }

        protected Webpage()
        {
            InheritFrontEndRolesFromParent = true;
            IncludeInSitemap = true;
            Urls = new List<UrlHistory>();
            Widgets = new List<Widget.Widget>();
            FrontEndAllowedRoles = new HashSet<UserRole>();
        }

        [Required]
        [Remote("ValidateUrlIsAllowed", "Webpage", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$",
            ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Url Segment")]
        public override string UrlSegment { get; set; }

        [StringLength(250, ErrorMessage = "SEO Target Phrase cannot be longer than 250 characters.")]
        [DisplayName("SEO Target Phrase")]
        public virtual string SEOTargetPhrase { get; set; }

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

        public virtual bool IncludeInSitemap { get; set; }

        [DisplayName("Custom header scripts")]
        [StringLength(8000)]
        public virtual string CustomHeaderScripts { get; set; }

        [DisplayName("Custom footer scripts")]
        [StringLength(8000)]
        public virtual string CustomFooterScripts { get; set; }


        [DisplayName("Requires SSL")]
        public virtual bool RequiresSSL { get; set; }

        public virtual bool Published { get; set; }

        public virtual WebpagePublishStatus PublishStatus
        {
            get
            {
                WebpagePublishStatus status = Published
                    ? WebpagePublishStatus.Published
                    : PublishOn.HasValue
                        ? WebpagePublishStatus.Scheduled
                        : WebpagePublishStatus.Unpublished;
                return status;
            }
        }

        public virtual string LiveUrlSegment
        {
            get { return CurrentRequestData.HomePage == this ? string.Empty : UrlSegment; }
        }

        [UIHint("DateTime")]
        [DisplayName("Publish On")]
        public virtual DateTime? PublishOn { get; set; }

        public virtual ISet<Widget.Widget> ShownWidgets { get; set; }
        public virtual ISet<Widget.Widget> HiddenWidgets { get; set; }

        public virtual IList<Widget.Widget> Widgets { get; set; }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        [AllowHtml]
        [DisplayName("Body Content")]
        public virtual string BodyContent { get; set; }

        public virtual IEnumerable<Webpage> ActivePages
        {
            get
            {
                Webpage page = this;
                while (page != null)
                {
                    yield return page;
                    page = page.Parent.Unproxy() as Webpage;
                }
            }
        }


        [DisplayName("Block Anonymous Access")]
        public virtual bool BlockAnonymousAccess { get; set; }

        //forms
        [DisplayName("Form Submitted Message")]
        [AllowHtml]
        [StringLength(500, ErrorMessage = "Form submitted messsage cannot be longer than 500 characters.")]
        public virtual string FormSubmittedMessage { get; set; }

        [DisplayName("Form Success Redirect")]
        public virtual string FormRedirectUrl { get; set; }

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
        [DisplayName("Submit button custom text")]
        public virtual string SubmitButtonText { get; set; }


        [DisplayName("Same as parent")]
        public virtual bool InheritFrontEndRolesFromParent { get; set; }

        public virtual ISet<UserRole> FrontEndAllowedRoles { get; set; }

        [DisplayName("Roles")]
        public virtual string FrontEndRoles
        {
            get { return string.Join(", ", FrontEndAllowedRoles.Select(x => x.Name)); }
        }

        public virtual string AbsoluteUrl
        {
            get
            {
                string scheme = (RequiresSSL || MrCMSApplication.Get<SiteSettings>().SSLEverywhere)
                    ? "https://"
                    : "http://";
                string authority = Site.BaseUrl;
                if (authority.EndsWith("/"))
                    authority = authority.TrimEnd('/');

                return string.Format("{0}{1}/{2}", scheme, authority, LiveUrlSegment);
            }
        }

        public virtual IList<UrlHistory> Urls { get; set; }

        public virtual PageTemplate PageTemplate { get; set; }
    }
}