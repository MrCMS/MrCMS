using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class RedirectMetadata : DocumentMetadataMap<Redirect>
    {
        public override string IconClass { get { return "icon-forward"; } }
        public override int DisplayOrder { get { return 6; } }
        public override string EditPartialView { get { return "RedirectEdit"; } }
        public override bool HasBodyContent { get { return false; }}
    }
}