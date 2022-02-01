using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Slider")]
public class Slider : IContentBlockWithChildCollection
{
    public Slider()
    {
        ShowIndicator = true;
        Interval = 5000;
    }
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

