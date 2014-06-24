using System.ComponentModel;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MediaCategorySearchModel
    {
        public MediaCategorySearchModel()
        {
            Page = 1;
        }

        public int Id { get; set; }
        public int Page { get; set; }
        [DisplayName("Search files")]
        public string SearchText { get; set; }

    }
}