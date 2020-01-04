using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using System;
using MrCMS.Data;

namespace MrCMS.Entities.Indexes
{
    public class AdminWebpageIndexDefinition : IndexDefinition<Webpage>
    {
        public AdminWebpageIndexDefinition(IRepository<Webpage> repository,
            IWebHostEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(repository, hostingEnvironment, serviceProvider)
        {
        }

        public override string IndexFolderName => "WebpagesAdmin";

        public override string IndexName => "Default Admin Webpage Index";

    }
}