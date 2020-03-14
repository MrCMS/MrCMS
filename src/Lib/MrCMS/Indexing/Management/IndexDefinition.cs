using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using Version = Lucene.Net.Util.LuceneVersion;

namespace MrCMS.Indexing.Management
{
    public abstract class IndexDefinition
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        protected IndexDefinition(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string SystemName => GetType().FullName;

        public abstract string IndexFolderName { get; }

        protected Analyzer Analyser;
        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(Version.LUCENE_48));
        }

        public abstract string IndexName { get; }

        public string GetLocation(Site site)
        {
            string location = string.Format("App_Data/Indexes/{0}/{1}/", site.Id, IndexFolderName);
            string mapPath = Path.Combine(_hostingEnvironment.ContentRootPath, location);
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
            {
                return new List<LuceneAction>();
            }

            var systemEntity = entity as SystemEntity;
            var actionsDictionary = GetActionsDictionary(operation);
            var luceneActions = new List<LuceneAction>();
            foreach (var action in actionsDictionary.Keys.SelectMany(key => actionsDictionary[key]))
            {
                luceneActions.AddRange(action(systemEntity));
            }
            return luceneActions;
        }

        public virtual string[] SearchableFieldNames => DefinitionInfos
                               .Select(info => info.Name)
                               .Distinct()
                               .ToArray();
    }

    public abstract class IndexDefinition<T> : IndexDefinition where T : SystemEntity
    {
        private static IEnumerable<string> GetEntityTypes(T entity)
        {
            if (entity == null)
            {
                yield break;
            }

            Type entityType = entity.GetType();
            while (typeof(T).IsAssignableFrom(entityType))
            {
                yield return entityType.FullName;
                entityType = entityType.BaseType;
            }
        }

        protected readonly IRepositoryBase<T> Repository;
        private readonly IServiceProvider _serviceProvider;

        protected IndexDefinition(IRepositoryBase<T> repository, IWebHostEnvironment hostingEnvironment, IServiceProvider serviceProvider)
        : base(hostingEnvironment)
        {
            Repository = repository;
            _serviceProvider = serviceProvider;
        }

        public static FieldDefinition<T> Id { get; } = new StringFieldDefinition<T>("id", entity => new List<string> { entity.Id.ToString() },
            entity => entity.ToDictionary(arg => arg, arg => new List<string> { arg.Id.ToString() }.AsEnumerable()),
            Field.Store.YES);

        public static FieldDefinition<T> EntityType { get; } = new StringFieldDefinition<T>("entityType", GetEntityTypes,
            entity => entity.ToDictionary(arg => arg, GetEntityTypes),
            Field.Store.YES);

        public Document Convert(T entity)
        {
            var document = new Document();
            document = document.SetFields(GetNewDefinitionList().Concat(Definitions), entity);

            foreach (var additionalField in GetAdditionalFields(entity))
            {
                document.Add(additionalField);
            }

            return document;
        }

        protected virtual IEnumerable<IIndexableField> GetAdditionalFields(T entity)
        {
            yield break;
        }

        protected virtual Dictionary<T, IEnumerable<IIndexableField>> GetAdditionalFields(List<T> entities)
        {
            return entities.ToDictionary(arg => arg, arg => Enumerable.Empty<IIndexableField>());
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
                    List<IIndexableField> abstractFields = fieldInfo[entity];
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

        public sealed override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get
            {
                var definitionInterfaceType = typeof(IFieldDefinition<,>).MakeGenericType(GetType(), typeof(T));
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(definitionInterfaceType);
                return types.Select(type => _serviceProvider.GetService(type)).OfType<IFieldDefinitionInfo>();

            }
        }


        public IEnumerable<FieldDefinition<T>> Definitions => DefinitionInfos.OfType<IFieldDefinition<T>>().Select(x => x.GetDefinition);

        public TFieldDefinition GetFieldDefinition<TFieldDefinition>() where TFieldDefinition : IFieldDefinitionInfo
        {
            return DefinitionInfos.OfType<TFieldDefinition>().FirstOrDefault();
        }

        public IEnumerable<string> FieldNames => Definitions.Select(x => x.FieldName);

        public sealed override Document Convert(object entity)
        {
            if (entity is T)
            {
                return Convert(entity as T);
            }

            return null;
        }

        public sealed override Term GetIndex(object entity)
        {
            if (entity is T)
            {
                return GetIndex(entity as T);
            }

            return null;
        }

        public sealed override IEnumerable<Type> GetUpdateTypes(LuceneOperation operation)
        {
            return GetActionsDictionary(operation).Keys;
        }

        public virtual Task<T> Convert(Document document)
        {
            return Repository.GetData(document.GetValue<int>(Id.FieldName));
        }

        public virtual async Task<IEnumerable<T>> Convert(IEnumerable<Document> documents)
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            var list = new List<T>();
            foreach (IEnumerable<int> ints in ids.Chunk(100))
            {
                list.AddRange(await Repository.Query<T>()
                    .Where(arg => ints.Contains(arg.Id))
                    //.Cacheable()
                    .ToListAsync());
            }

            return list.OrderBy(x => ids.IndexOf(x.Id)).ToList();
        }
        public virtual async Task<IEnumerable<T2>> Convert<T2>(IEnumerable<Document> documents) where T2 : T
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            var list = new List<T2>();
            foreach (IEnumerable<int> ints in ids.Chunk(100))
            {
                list.AddRange(await Repository.Query<T2>()
                    .Where(arg => ints.Contains(arg.Id))
                    //.Cacheable()
                    .ToListAsync());
            }

            return list.OrderBy(x => ids.IndexOf(x.Id)).ToList();
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
            {
                yield return new LuceneAction
                {
                    Entity = arg,
                    IndexDefinition = this,
                    Operation = operation,
                };
            }
        }
    }
}