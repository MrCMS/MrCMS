using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Indexing
{
    public class WebpageSearchIndexDefinition : IndexDefinition<Webpage>
    {
        private readonly HashSet<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>> _definitions;

        public WebpageSearchIndexDefinition(ISession session, IGetLuceneIndexSearcher getLuceneIndexSearcher,
            IEnumerable<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>> definitions,
            IHostingEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(session, getLuceneIndexSearcher,hostingEnvironment, serviceProvider)
        {
            _definitions = new HashSet<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>>(definitions);
        }

        public override string IndexFolderName
        {
            get { return "UI Webpages"; }
        }

        public override string IndexName
        {
            get { return "UI Webpage Index"; }
        }
    }
}