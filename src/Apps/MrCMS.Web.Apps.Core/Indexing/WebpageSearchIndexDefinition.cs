using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;
using System;

namespace MrCMS.Web.Apps.Core.Indexing
{
    public class WebpageSearchIndexDefinition : IndexDefinition<Webpage>
    {
        public WebpageSearchIndexDefinition(ISession session,
            IHostingEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(session, hostingEnvironment, serviceProvider)
        {
        }

        public override string IndexFolderName => "UI Webpages";

        public override string IndexName => "UI Webpage Index";
    }
}