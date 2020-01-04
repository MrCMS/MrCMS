using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using System;
using MrCMS.Data;

namespace MrCMS.Web.Apps.Core.Indexing
{
    public class WebpageSearchIndexDefinition : IndexDefinition<Webpage>
    {
        public WebpageSearchIndexDefinition(IRepository<Webpage> repository,
            IWebHostEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(repository, hostingEnvironment, serviceProvider)
        {
        }

        public override string IndexFolderName => "UI Webpages";

        public override string IndexName => "UI Webpage Index";
    }
}