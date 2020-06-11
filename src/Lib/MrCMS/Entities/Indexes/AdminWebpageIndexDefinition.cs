using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;
using System;

namespace MrCMS.Entities.Indexes
{
    public class AdminWebpageIndexDefinition : IndexDefinition<Webpage>
    {
        public AdminWebpageIndexDefinition(ISession session,
            IWebHostEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(session, hostingEnvironment, serviceProvider)
        {
        }

        public override string IndexFolderName => "WebpagesAdmin";

        public override string IndexName => "Default Admin Webpage Index";

    }
}