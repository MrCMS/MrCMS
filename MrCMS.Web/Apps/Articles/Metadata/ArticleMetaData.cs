using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class ArticleMetaData : DocumentMetadataMap<Article>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override bool RequiresParent
        {
            get { return true; }
        }

        public override bool AutoBlacklist
        {
            get { return true; }
        }

        public override string DefaultLayoutName
        {
            get { return "Two Column"; }
        }
        
        public override bool Sortable
        {
            get { return false; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override bool RevealInNavigation
        {
            get { return false; }
        }
    }
}

