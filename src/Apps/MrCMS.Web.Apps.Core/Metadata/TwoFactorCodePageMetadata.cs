using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class TwoFactorCodePageMetadata : DocumentMetadataMap<TwoFactorCodePage>
    {
        public override string IconClass => "fa fa-user";

        public override string WebGetController => "TwoFactorCodePage";

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;
    }
}