using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public enum MediaCategorySortMethod
    {
        [Display(Name = "Created On Descending")]
        CreatedOnDesc,
        [Display(Name = "Created On")]
        CreatedOn,
        [Display(Name = "Name")]
        Name,
        [Display(Name = "Name Descending")]
        NameDesc,
        [Display(Name = "Display Order")]
        DisplayOrder,
        [Display(Name = "Reverse Display Order")]
        DisplayOrderDesc,
    }
}