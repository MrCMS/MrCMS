using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class UserAccountMetaData : DocumentMetadataMap<UserAccountPage>
    {
        public override string IconClass
        {
            get { return "icon-user"; }
        }

        public override string WebGetAction
        {
            get { return "Show"; }
        }
        public override string WebGetController
        {
            get
            {
                return "UserAccount";
            }
        }
    }
}