using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class WebpagePasswordPageMetadata : WebpageMetadataMap<WebpagePasswordPage>
    {
        public override string WebGetController => nameof(WebpagePasswordPage);

        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }
    }
}