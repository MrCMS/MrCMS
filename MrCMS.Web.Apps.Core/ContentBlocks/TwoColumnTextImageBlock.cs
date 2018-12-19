using MrCMS.Entities.Documents;

namespace MrCMS.Web.Apps.Core.ContentBlocks
{
    public class TwoColumnTextImageBlock : ContentBlock
    {
        public virtual string Text { get; set; }
        public virtual string Image { get; set; }
        public virtual string Text2 { get; set; }
        public virtual string Image2 { get; set; }
        public virtual string Link { get; set; }
        public virtual string Link2 { get; set; }
    }
}