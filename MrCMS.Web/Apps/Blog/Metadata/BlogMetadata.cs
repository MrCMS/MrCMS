using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Blog.Pages;

namespace MrCMS.Web.Apps.Blog.Metadata
{
    public class BlogMetadata : DocumentMetadataMap<Pages.Blog>
    {
        public override IEnumerable<Type> ChildrenList { get { yield return typeof(Post); } }
        public override ChildrenListType ChildrenListType { get { return ChildrenListType.WhiteList; } }
    }
}