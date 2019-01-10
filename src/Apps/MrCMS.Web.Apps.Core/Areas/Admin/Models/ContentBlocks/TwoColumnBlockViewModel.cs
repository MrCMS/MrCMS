using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class TwoColumnBlockViewModel : IAddPropertiesViewModel<TwoColumnBlock>, IUpdatePropertiesViewModel<TwoColumnBlock>
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
    }
}