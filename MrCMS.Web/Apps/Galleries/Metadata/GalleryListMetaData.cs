using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Galleries.Pages;

namespace MrCMS.Web.Apps.Galleries.Metadata
{
    public class GalleryListMetaData : DocumentMetadataMap<GalleryList>
    {
        public override string IconClass
        {
            get
            {
                return "icon-th";
            }
        }
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(Pages.Gallery); }
        }

        //sort child items by the display order in web tree
        public override Func<Document, object> SortBy
        {
            get
            {
                return document => document.DisplayOrder;
            }
        }

        public override string WebGetController
        {
            get { return "GalleryList"; }
        }
        public override string WebGetAction
        {
            get { return "View"; }
        }

    }
}

