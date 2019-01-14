using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaCategory : Document
    {
        [Required]
        [Remote("ValidateUrlIsAllowed", "MediaCategory", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage =
            "Path must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Path")]
        public override string UrlSegment { get; set; }

        public virtual string MetaTitle { get; set; }
        public virtual string MetaDescription { get; set; }

        [DisplayName("Allow use as gallery")]
        public virtual bool IsGallery { get; set; }

        public virtual IList<MediaFile> Files { get; protected internal set; } = new List<MediaFile>();
    }
}