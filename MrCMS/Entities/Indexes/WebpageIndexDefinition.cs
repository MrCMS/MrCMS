using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Website;
using NHibernate;
using MrCMS.Indexing.Utils;
using System.Linq;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Entities.Indexes
{
    public class WebpageIndexDefinition : IIndexDefinition<Webpage>
    {
        private Analyzer _analyser;

        public Document Convert(Webpage entity)
        {
            var document = new Document();
            document.AddField("id", entity.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)
                    .AddField("name", entity.Name, Field.Store.YES, Field.Index.ANALYZED)
                    .AddField("bodycontent", entity.BodyContent, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metatitle", entity.MetaTitle, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metakeywords", entity.MetaKeywords, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metadescription", entity.MetaDescription, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("urlsegment", entity.UrlSegment, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("publishOn",
                              entity.PublishOn.HasValue
                                  ? DateTools.DateToString(entity.PublishOn.Value, DateTools.Resolution.SECOND)
                                  : null, Field.Store.NO, Field.Index.NOT_ANALYZED);
            return document;
        }

        public Term GetIndex(Webpage entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        public Webpage Convert(ISession session, Document document)
        {
            return session.Get<Webpage>(document.GetValue<int>("id"));
        }

        public IEnumerable<Webpage> Convert(ISession session, IEnumerable<Document> documents)
        {
            var ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                ids.Chunk(100)
                   .SelectMany(
                       ints => session.QueryOver<Webpage>().Where(webpage => webpage.Id.IsIn(ints.ToList())).List());
        }

        public string GetLocation(CurrentSite currentSite)
        {
            var location = string.Format("~/App_Data/Indexes/{0}/Webpages/", currentSite.Id);
            var mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return _analyser ?? (_analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        public string Name { get { return "Default Webpage Index"; } }
    }
}