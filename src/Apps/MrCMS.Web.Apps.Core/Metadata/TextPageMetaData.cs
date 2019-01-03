using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class TextPageMetaData : DocumentMetadataMap<TextPage>
    {
        public override string IconClass => "fa fa-book";
        public override string WebGetController => "TextPage";
    }
}