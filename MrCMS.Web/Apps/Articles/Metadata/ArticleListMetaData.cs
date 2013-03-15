using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class ArticleListMetaData : DocumentMetadataMap<ArticleList>
    {
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
            get { yield return typeof(Pages.Article); }
        }

        public override bool SortByDesc
        {
            get { return true; }
        }

        public override Func<Document, object> SortBy
        {
            get
            {
                return document => document.CreatedOn;
            }
        }

        public override int MaxChildNodes
        {
            get { return 5; }
        }

        public override string WebGetController
        {
            get { return "ArticleList"; }
        }
        public override string WebGetAction
        {
            get { return "View"; }
        }
    }
}

