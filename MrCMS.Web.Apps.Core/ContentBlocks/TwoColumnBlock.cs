using MrCMS.Entities.Documents;

namespace MrCMS.Web.Apps.Core.ContentBlocks
{
    public class TwoColumnBlock : ContentBlock
    {
        public virtual string Text { get; set; }
        public virtual string Text2 { get; set; }
    }
}