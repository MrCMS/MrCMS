using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaCategory : Document
    {
        [Required]
        [Remote("ValidateUrlIsAllowed", "MediaCategory", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage = "Path must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Path")]
        public override string UrlSegment { get; set; }

        public virtual string MetaTitle { get; set; }
        public virtual string MetaDescription { get; set; }
        [DisplayName("Allow use as gallery")]
        public virtual bool IsGallery { get; set; }

        private IList<MediaFile> _files = new List<MediaFile>();

        public virtual IList<MediaFile> Files
        {
            get { return _files; }
            protected internal set { _files = value; }
        }

        public virtual IEnumerable<MediaCategory> BreadCrumbs
        {
            get
            {
                MediaCategory page = this;
                while (page != null)
                {
                    yield return page;
                    page = page.Parent.Unproxy() as MediaCategory;
                }
            }
        }
    }
}
