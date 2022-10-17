using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;

public class UpdateImageGalleryAdminModel
{
    [DisplayName("Responsive Classes")]
    public string ResponsiveClasses { get; set; }
    
    public string ImageRatio { get; set; }
    
    public int ImageRenderSize { get; set; }
}