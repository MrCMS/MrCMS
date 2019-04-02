using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class ArticleMetadata : DocumentMetadataMap<Article>
    {
        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;

        public override bool RequiresParent => true;

        public override bool AutoBlacklist => true;

        public override bool Sortable => false;

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override bool RevealInNavigation => false;
    }
}
