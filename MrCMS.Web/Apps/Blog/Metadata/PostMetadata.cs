using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Blog.Pages;

namespace MrCMS.Web.Apps.Blog.Metadata
{
    public class PostMetadata : DocumentMetadataMap<Post>
    {
        public override ChildrenListType ChildrenListType { get { return ChildrenListType.WhiteList; } }
        public override bool RequiresParent { get { return true; } }
    }
}