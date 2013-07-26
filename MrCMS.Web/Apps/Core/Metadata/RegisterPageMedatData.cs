using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class RegisterPageMedatData : DocumentMetadataMap<SearchPage>
    {
        public override string IconClass
        {
            get { return "icon-search";}
        }
        public override string WebGetController
        {
            get { return "SearchPage"; }
        }
        public override string WebPostController
        {
            get { return "SearchPage"; }
        }
        public override bool HasBodyContent { get { return false; } }
    }
}