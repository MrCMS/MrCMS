using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Slider")]
public class Slider : IContentBlockWithChildCollection
{
    public IReadOnlyList<BlockItem> Items => Slides;
    public List<Slide> Slides { get; set; } = new();

    public int Interval { get; set; }

    public bool ShowIndicator { get; set; }

    public bool PauseOnHover { get; set; }

    public BlockItem AddChild()
    {
        var slide = new Slide();
        Slides.Add(slide);
        return slide;
    }

    public void Remove(BlockItem item)
    {
        Slides.Remove(item as Slide);
    }
}

