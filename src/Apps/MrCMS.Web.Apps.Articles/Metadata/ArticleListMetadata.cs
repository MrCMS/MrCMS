using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class ArticleListMetadata : DocumentMetadataMap<ArticleList>
    {
        public override string WebGetController => "ArticleList";

        public override string IconClass => "fa fa-list";

        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(Article); }
        }

        public override SortBy SortBy => SortBy.PublishedOnDesc;

        public override int MaxChildNodes => 5;
    }
}
