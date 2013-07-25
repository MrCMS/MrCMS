using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class ForgottenPasswordMetaData : DocumentMetadataMap<ForgottenPasswordPage>
    {
        public override string IconClass
        {
            get { return "icon-user"; }
        }
        public override string WebGetController
        {
            get { return "Login"; }
        }
        public override string WebPostController
        {
            get { return "Login"; }
        }
        public override string WebGetAction
        {
            get { return "ForgottenPassword"; }
        }
        public override string WebPostAction
        {
            get { return "ForgottenPassword"; }
        }

    }
}