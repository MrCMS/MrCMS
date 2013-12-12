using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Indexing.Management
{
    public abstract class IndexDefinition<T> : IIndexDefinition<T> where T : SystemEntity
    {
        protected Analyzer _analyser;
        public Document Convert(T entity)
        {
            return new Document().SetFields(new List<FieldDefinition<T>> { Id }.Concat(Definitions), entity);
        }

        public Term GetIndex(T entity)
        {
            return new Term(Id.FieldName, entity.Id.ToString());
        }
        public virtual T Convert(ISession session, Document document)
        {
            return session.Get<T>(document.GetValue<int>(Id.FieldName));
        }
        public virtual IEnumerable<T> Convert(ISession session, IEnumerable<Document> documents)
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                ids.Chunk(100)
                   .SelectMany(
                       ints =>
                       session.QueryOver<T>()
                              .Where(arg => arg.Id.IsIn(ints.ToList()))
                              .Cacheable()
                              .List()
                              .OrderBy(arg => ids.IndexOf(arg.Id)));
        }
        public string GetLocation(Site currentSite)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/{1}/", currentSite.Id, IndexFolderName);
            string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public abstract string IndexFolderName { get; }

        public virtual Analyzer GetAnalyser()
        {
            return _analyser ?? (_analyser = new StandardAnalyzer(Version.LUCENE_30));
        }
        public abstract IEnumerable<FieldDefinition<T>> Definitions { get; }
        public abstract string IndexName { get; }

        private static readonly FieldDefinition<T> _id =
            new StringFieldDefinition<T>("id", entity => entity.Id.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        public static FieldDefinition<T> Id { get { return _id; } }
    }
}