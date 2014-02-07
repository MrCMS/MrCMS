using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class CompleteExternalRegistrationPageMetadata : DocumentMetadataMap<CompleteExternalRegistrationPage>
    {
        public override string IconClass
        {
            get { return "icon-user"; }
        }
        public override string WebGetController
        {
            get { return "CompleteExternalRegistrationPage"; }
        }
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield break;
            }
        }
    }
}