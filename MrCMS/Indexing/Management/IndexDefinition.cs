using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Entities.Indexes;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Id.Insert;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Indexing.Management
{
    public abstract class IndexDefinition
    {
        public string SystemName
        {
            get { return GetType().FullName; }
        }

        public abstract string IndexFolderName { get; }

        protected Analyzer Analyser;
        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(Version.LUCENE_30));
        }
        public abstract string IndexName { get; }

        public string GetLocation(Site site)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/{1}/", site.Id, IndexFolderName);
            string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public abstract Document Convert(object entity);
        public abstract Term GetIndex(object entity);
        public abstract IEnumerable<IFieldDefinitionInfo> DefinitionInfos { get; }
        public abstract IEnumerable<Type> GetUpdateTypes(LuceneOperation operation);
        public abstract Dictionary<Type, List<Func<SystemEntity, IEnumerable<LuceneAction>>>> GetActionsDictionary(
            LuceneOperation operation);

        public List<LuceneAction> GetAllActions(object entity, LuceneOperation operation)
        {
            if (!(entity is SystemEntity))
                return new List<LuceneAction>();
            var systemEntity = entity as SystemEntity;
            var actionsDictionary = GetActionsDictionary(operation);
            var luceneActions = new List<LuceneAction>();
            foreach (var action in actionsDictionary.Keys.SelectMany(key => actionsDictionary[key]))
            {
                luceneActions.AddRange(action(systemEntity));
            }
            return luceneActions;
        }
    }

    public abstract class IndexDefinition<T> : IndexDefinition where T : SystemEntity
    {
        private static readonly FieldDefinition<T> _id =
            new StringFieldDefinition<T>("id", entity => entity.Id.ToString(), Field.Store.YES,
                Field.Index.NOT_ANALYZED);
        protected readonly ISession _session;
        protected IndexDefinition(ISession session)
        {
            _session = session;
        }

        public static FieldDefinition<T> Id
        {
            get { return _id; }
        }

        public Document Convert(T entity)
        {
            return new Document().SetFields(new List<FieldDefinition<T>> { Id }.Concat(Definitions), entity);
        }

        public Term GetIndex(T entity)
        {
            return new Term(Id.FieldName, entity.Id.ToString());
        }

        public abstract IEnumerable<FieldDefinition<T>> Definitions { get; }
        public abstract IEnumerable<string> FieldNames { get; }

        public sealed override Document Convert(object entity)
        {
            if (entity is T)
                return Convert(entity as T);
            return null;
        }

        public sealed override Term GetIndex(object entity)
        {
            if (entity is T)
                return GetIndex(entity as T);
            return null;
        }

        public sealed override IEnumerable<Type> GetUpdateTypes(LuceneOperation operation)
        {
            return GetActionsDictionary(operation).Keys;
        }

        public virtual T Convert(Document document)
        {
            return _session.Get<T>(document.GetValue<int>(Id.FieldName));
        }

        public virtual IEnumerable<T> Convert(IEnumerable<Document> documents)
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                ids.Chunk(100)
                    .SelectMany(
                        ints =>
                            _session.QueryOver<T>()
                                .Where(arg => arg.Id.IsIn(ints.ToList()))
                                .Cacheable()
                                .List()
                                .OrderBy(arg => ids.IndexOf(arg.Id)));
        }

        public sealed override Dictionary<Type, List<Func<SystemEntity, IEnumerable<LuceneAction>>>> GetActionsDictionary(LuceneOperation operation)
        {
            var actionsDictionary = new Dictionary<Type, List<Func<SystemEntity, IEnumerable<LuceneAction>>>>
                                    {
                                        {
                                            typeof (T),
                                            new List<Func<SystemEntity, IEnumerable<LuceneAction>>>
                                            {
                                                entity => GetRootEntityAction(entity, operation)
                                            }
                                        }
                                    };
            foreach (var fieldDefinitionInfo in DefinitionInfos)
            {
                AppendActions(ref actionsDictionary, fieldDefinitionInfo);
            }
            return actionsDictionary;
        }

        private void AppendActions(ref Dictionary<Type, List<Func<SystemEntity, IEnumerable<LuceneAction>>>> actionsDictionary, IFieldDefinitionInfo fieldDefinitionInfo)
        {
            var relatedEntities = fieldDefinitionInfo.GetRelatedEntities();
            foreach (var key in relatedEntities.Keys)
            {
                if (actionsDictionary.ContainsKey(key))
                {
                    actionsDictionary[key].Add(relatedEntities[key]);
                }
                else
                {
                    actionsDictionary[key] = new List<Func<SystemEntity, IEnumerable<LuceneAction>>>
                                             {
                                                 relatedEntities[key]
                                             };
                }
            }
        }

        private IEnumerable<LuceneAction> GetRootEntityAction(SystemEntity arg, LuceneOperation operation)
        {
            if (arg is T)
                yield return new LuceneAction
                             {
                                 Entity = arg,
                                 IndexDefinition = this,
                                 Operation = operation,
                             };
        }
    }
}