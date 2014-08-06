using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Galleries.Pages;

namespace MrCMS.Web.Apps.Galleries.Metadata
{
    public class GalleryMetaData : DocumentMetadataMap<Gallery>
    {
        public override string IconClass
        {
            get
            {
                return "glyphicon glyphicon-picture";
            }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }
    }
}

