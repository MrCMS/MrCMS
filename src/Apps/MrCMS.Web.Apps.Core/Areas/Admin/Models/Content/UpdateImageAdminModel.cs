using static MrCMS.Web.Apps.Core.Entities.ContentBlocks.Image;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;

public class UpdateImageAdminModel
{
    public string Url { get; set; }
    public ImageAligment Aligment { get; set; }
    public string cssClasses { get; set; }
}