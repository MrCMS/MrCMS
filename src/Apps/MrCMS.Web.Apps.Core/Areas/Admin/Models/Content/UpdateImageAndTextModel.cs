using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;

public class UpdateImageAndTextModel
{
    public UpdateImageAndTextModel()
    {
        ContentVerticalAlignment = ImageAndText.VerticalAlignment.Center;
    }
    public ImageAndText.ImageAndTextLayout Layout { get; set; }
    public ImageAndText.VerticalAlignment ContentVerticalAlignment { get; set; }
}