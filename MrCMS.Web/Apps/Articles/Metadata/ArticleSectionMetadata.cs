using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class ArticleSectionMetadata : DocumentMetadataMap<ArticleSection>
    {
        public override string WebGetController
        {
            get { return "ArticleSection"; }
        }
        public override string IconClass
        {
            get { return "icon-list"; }
        }
        
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield return typeof (Article);
                yield return typeof (ArticleSection);
            }
        }

        public override string DefaultLayoutName
        {
            get { return "Two Column"; }
        }

        public override SortBy SortBy
        {
            get
            {
                return SortBy.PublishedOnDesc;
            }
        }

        public override int MaxChildNodes
        {
            get { return 25; }
        }
    }
}

