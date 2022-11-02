using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Slider")]
public class Slider : IContentBlockWithSortableChildCollection
{
    public Slider()
    {
        ShowIndicator = true;
        Interval = 5000;
        CaptionCssClass = "d-none d-md-block";
    }
    public IReadOnlyList<BlockItem> Items => Slides;
    public List<Slide> Slides { get; set; } = new();

    public int Interval { get; set; }

    public bool ShowIndicator { get; set; }

    public bool PauseOnHover { get; set; }
    public string CaptionCssClass { get; set; }

    public BlockItem AddChild()
    {
        var slide = new Slide();

        slide.Order = Slides.Count + 1;
        
        Slides.Add(slide);
        return slide;
    }

    public void Remove(BlockItem item)
    {
        Slides.Remove(item as Slide);
    }

    public void Sort(List<KeyValuePair<Guid, int>> OrderedIds)
    {
        foreach (var slide in Slides)
        {
            slide.Order = OrderedIds.FirstOrDefault(f => f.Key == slide.Id).Value;
        }
        Slides = Slides.OrderBy(f => f.Order).ToList();
    }
}

