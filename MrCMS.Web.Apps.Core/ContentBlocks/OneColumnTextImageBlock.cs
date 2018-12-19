using MrCMS.Entities.Documents;

namespace MrCMS.Web.Apps.Core.ContentBlocks
{
    public class OneColumnTextImageBlock : ContentBlock
    {
        public virtual string Text { get; set; }
        public virtual string Image { get; set; }
        public virtual string Link { get; set; }
    }
}