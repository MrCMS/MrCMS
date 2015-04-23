using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MediaCategorySearchModel
    {
        public MediaCategorySearchModel()
        {
            Page = 1;
        }

        public int? Id { get; set; }
        public int Page { get; set; }

        [DisplayName("Search")]
        public string SearchText { get; set; }

        public MediaCategorySortMethod SortBy { get; set; }
    }

    public enum MediaCategorySortMethod
    {
        [Display(Name = "Created On Descending")]
        CreatedOnDesc,
        [Display(Name = "Created On")]
        CreatedOn,
        [Display(Name = "Display Order Descending")]
        DisplayOrderDesc,
        [Display(Name = "Display Order")]
        DisplayOrder,
    }
}