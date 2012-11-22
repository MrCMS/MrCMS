using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddListingPage : Webpage
    {
        public int MediaCategoryId { get; set; }
        public MediaCategory MediaCategory { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> MediaCategories { get; set; }
        public virtual IEnumerable<ImageDetail> Images { get; set; }
    }

    public class ImageDetail : BaseEntity
    {
        public virtual MediaFile MediaFile { get; set; }
        public virtual string Url { get; set; }
    }
}