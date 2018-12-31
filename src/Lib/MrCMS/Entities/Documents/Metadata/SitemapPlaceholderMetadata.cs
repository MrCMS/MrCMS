using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class SitemapPlaceholderMetadata : DocumentMetadataMap<SitemapPlaceholder>
    {
        public override string IconClass { get { return "glyphicon glyphicon-forward"; } }
        public override int DisplayOrder { get { return 99; } }
        public override bool HasBodyContent { get { return false; } }

        public override string WebGetController
        {
            get { return "SitemapPlaceholder"; }
        }
    }
}