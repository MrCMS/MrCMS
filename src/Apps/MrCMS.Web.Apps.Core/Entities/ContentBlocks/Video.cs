using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Video")]
public class Video : IContentBlock
{
    public VideoType ActiveType { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { Youtube, Embed };

    public YoutubeVideo Youtube { get; set; } = new() { Name = "Youtube" };

    public EmbedVideo Embed { get; set; } = new() { Name = "Embed" };

    public enum VideoType
    {
        Yotube,
        EmbedVideo
    }
}