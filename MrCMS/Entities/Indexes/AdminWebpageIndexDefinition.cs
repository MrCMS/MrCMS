using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Entities.Indexes
{
    public class AdminWebpageIndexDefinition : IndexDefinition<Webpage>
    {

        public AdminWebpageIndexDefinition(ISession session, IGetLuceneIndexSearcher getLuceneIndexSearcher,
            IHostingEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(session, getLuceneIndexSearcher, hostingEnvironment, serviceProvider)
        {
        }


        public override string IndexFolderName
        {
            get { return "WebpagesAdmin"; }
        }

        public override string IndexName
        {
            get { return "Default Admin Webpage Index"; }
        }

    }
}