using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using Ninject.Infrastructure.Language;
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

        public virtual string[] SearchableFieldNames
        {
            get
            {
                return DefinitionInfos.Where(
                    info => info.Index == Field.Index.ANALYZED || info.Index == Field.Index.ANALYZED_NO_NORMS)
                               .Select(info => info.Name)
                               .Distinct()
                               .ToArray();
            }
        }
    }

    public abstract class IndexDefinition<T> : IndexDefinition where T : SystemEntity
    {
        private static readonly FieldDefinition<T> _id =
            new StringFieldDefinition<T>("id", entity => new List<string> { entity.Id.ToString() }.ToEnumerable(),
                entity => entity.ToDictionary(arg => arg, arg => new List<string> { arg.Id.ToString() }.ToEnumerable()),
                Field.Store.YES,
                Field.Index.NOT_ANALYZED);
        private static readonly FieldDefinition<T> _entityType =
            new StringFieldDefinition<T>("entityType", GetEntityTypes,
                entity => entity.ToDictionary(arg => arg, GetEntityTypes),
                Field.Store.YES,
                Field.Index.NOT_ANALYZED);

        private static IEnumerable<string> GetEntityTypes(T entity)
        {
            if (entity == null)
                yield break;
            Type entityType = entity.GetType();
            while (typeof(T).IsAssignableFrom(entityType))
            {
                yield return entityType.FullName;
                entityType = entityType.BaseType;
            }
        }

        protected readonly ISession _session;
        private readonly IGetLuceneIndexSearcher _getLuceneIndexSearcher;

        protected IndexDefinition(ISession session, IGetLuceneIndexSearcher getLuceneIndexSearcher)
        {
            _session = session;
            _getLuceneIndexSearcher = getLuceneIndexSearcher;
        }

        public static FieldDefinition<T> Id
        {
            get { return _id; }
        }
        public static FieldDefinition<T> EntityType
        {
            get { return _entityType; }
        }

        public Document Convert(T entity)
        {
            var document = new Document();
            document = document.SetFields(GetNewDefinitionList().Concat(Definitions), entity);

            foreach (var additionalField in GetAdditionalFields(entity))
                document.Add(additionalField);

            return document;
        }

        protected virtual IEnumerable<IFieldable> GetAdditionalFields(T entity)
        {
            yield break;
        }

        protected virtual Dictionary<T, IEnumerable<IFieldable>> GetAdditionalFields(List<T> entities)
        {
            return entities.ToDictionary(arg => arg, arg => Enumerable.Empty<IFieldable>());
        }

        private static List<FieldDefinition<T>> GetNewDefinitionList()
        {
            return new List<FieldDefinition<T>> { Id, EntityType };
        }

        public List<Document> ConvertAll(List<T> entities)
        {
            var fieldDefinitions = GetNewDefinitionList();
            fieldDefinitions.AddRange(Definitions);
            var list = fieldDefinitions.Select(fieldDefinition => fieldDefinition.GetFields(entities)).ToList();
            var additionalFields = GetAdditionalFields(entities);
            var documents = new List<Document>();
            foreach (var entity in entities)
            {
                var document = new Document();
                foreach (var fieldInfo in list)
                {
                    List<AbstractField> abstractFields = fieldInfo[entity];
                    abstractFields.ForEach(document.Add);
                }
                if (additionalFields.ContainsKey(entity))
                {
                    foreach (var additionalField in additionalFields[entity])
                    {
                        document.Add(additionalField);
                    }
                }
                documents.Add(document);
            }

            return documents;
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
        public virtual IEnumerable<T2> Convert<T2>(IEnumerable<Document> documents) where T2 : T
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                ids.Chunk(100)
                    .SelectMany(
                        ints =>
                            _session.QueryOver<T2>()
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