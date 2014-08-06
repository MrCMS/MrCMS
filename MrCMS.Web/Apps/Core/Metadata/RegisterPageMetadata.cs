using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class RegisterPageMetadata : DocumentMetadataMap<RegisterPage>
    {
        public override string IconClass
        {
            get { return "icon-user"; }
        }

        public override string WebGetController
        {
            get { return "Registration"; }
        }

        public override string WebPostController
        {
            get { return "Registration"; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }
    }
}