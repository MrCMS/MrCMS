using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class PlainBlockViewModel : IAddPropertiesViewModel<PlainBlock>, IUpdatePropertiesViewModel<PlainBlock>
    {
        public string Text { get; set; }
    }
}