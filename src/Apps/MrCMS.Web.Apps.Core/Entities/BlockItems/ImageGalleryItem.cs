using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;
using System.Linq;

namespace MrCMS.Web.Apps.Core.Entities.BlockItems;

[Display(Name = "Image")]
public class ImageGalleryItem : BlockItem
{
    public string Url { get; set; }

    public override string GetDisplayName(IContentBlock block)
    {
        var indexOf = block.Items.ToList().IndexOf(this);
        if (indexOf > -1)
            return $"Image #{indexOf + 1}";
        return "Image";
    }
}