using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Utils;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using EnumerableHelper = MrCMS.Helpers.EnumerableHelper;

namespace MrCMS.Entities.Indexes
{
    public class DocumentIndexDefinition : IIndexDefinition<Documents.Document>
    {
        private Analyzer _analyser;
        private FieldDefinition<Documents.Document> _id;
        private FieldDefinition<Documents.Document> _name;
        private FieldDefinition<Documents.Document> _type;
        private FieldDefinition<Documents.Document> _urlSegment;

        public Documents.Document Convert(ISession session, Document document)
        {
            return session.Get<Documents.Document>(document.GetValue<int>("id"));
        }

        public IEnumerable<Documents.Document> Convert(ISession session, IEnumerable<Document> documents)
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                EnumerableHelper.Chunk(ids, 100)
                   .SelectMany(
                       ints =>
                       session.QueryOver<Documents.Document>().Where(webpage => webpage.Id.IsIn(ints.ToList())).List());
        }

        public string GetLocation(CurrentSite currentSite)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/Documents/", currentSite.Id);
            string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return _analyser ?? (_analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        public IEnumerable<FieldDefinition<Documents.Document>> Definitions
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return Type;
                yield return UrlSegment;
            }
        }

        public FieldDefinition<Documents.Document> Id { get { return _id; } }
        public FieldDefinition<Documents.Document> Name { get { return _name; } }
        public FieldDefinition<Documents.Document> Type { get { return _type; } }
        public FieldDefinition<Documents.Document> UrlSegment { get { return _urlSegment; } }



        public string IndexName
        {
            get { return "Default Document Index"; }
        }

        public Document Convert(Documents.Document entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public Term GetIndex(Documents.Document entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        private string GetDocumentType(Documents.Document document)
        {
            switch (document.Unproxy().GetType().Name)
            {
                case "Layout":
                    return "Layout";
                case "MediaCategory":
                    return "MediaCategory";
                default:
                    return "Webpage";
            }
        }
    }
}