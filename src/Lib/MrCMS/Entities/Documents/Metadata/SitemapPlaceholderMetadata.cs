using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class SitemapPlaceholderMetadata : DocumentMetadataMap<SitemapPlaceholder>
    {
        public override string IconClass => "fa fa-forward";
        public override int DisplayOrder => 99;
        public override bool HasBodyContent => false;

        public override string WebGetController => "SitemapPlaceholder";
    }
}