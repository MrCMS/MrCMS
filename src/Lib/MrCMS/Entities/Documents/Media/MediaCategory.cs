using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaCategory : SiteEntity
    {
        [Required] [StringLength(255)] public virtual string Name { get; set; }
        public virtual MediaCategory Parent { get; set; }

        
        [Required]
        [DisplayName("Display Order")]
        public virtual int DisplayOrder { get; set; }

        public virtual bool HideInAdminNav { get; set; }
        [Required]
        [Remote("ValidateUrlIsAllowed", "MediaCategory", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage =
            "Path must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Path")]
        public virtual string Path { get; set; }

        public virtual string MetaTitle { get; set; }
        public virtual string MetaDescription { get; set; }

        [DisplayName("Allow use as gallery")]
        public virtual bool IsGallery { get; set; }

        public virtual IList<MediaFile> Files { get; protected internal set; } = new List<MediaFile>();
        
        public virtual IEnumerable<MediaCategory> BreadCrumbs
        {
            get
            {
                MediaCategory page = this;
                while (page != null)
                {
                    yield return page;
                    page = page.Parent.Unproxy();
                }
            }
        }

    }
}