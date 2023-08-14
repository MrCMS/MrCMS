using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class Video : IContentBlock
{
    public string DisplayName => $"Video ({ActiveType})";
    public VideoType ActiveType { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { Youtube, Embed };

    public YoutubeVideo Youtube { get; set; } = new() { Name = "Youtube" };

    public EmbedVideo Embed { get; set; } = new() { Name = "Embed" };

    public enum VideoType
    {
        Youtube,
        EmbedVideo
    }
}