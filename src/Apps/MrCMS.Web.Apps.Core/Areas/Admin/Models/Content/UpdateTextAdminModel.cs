using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;

public class UpdateTextAdminModel
{
    public string Heading { get; set; }
    public Text.Aligment HeadingAligment { get; set; }
    public string Subtext { get; set; }
}