using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class DocumentMetadataService : IDocumentMetadataService
    {
        private readonly IServiceProvider _serviceProvider;

        public DocumentMetadataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<DocumentMetadataInfo> _documentMetadataInfos;
        private List<DocumentMetadata> _documentMetadata;

        public IEnumerable<DocumentMetadata> WebpageMetadata
        {
            get { return GetDocumentMetadatas().Where(x => x.Type != null && x.Type.IsSubclassOf(typeof(Webpage))); }
        }

        public List<DocumentMetadata> GetDocumentMetadatas()
        {
            return _documentMetadata ??= DocumentMetadataInfos.Select(GetMetadata).ToList();
        }

        public List<DocumentMetadataInfo> DocumentMetadataInfos => _documentMetadataInfos ??= GetDocumentMetadataInfo();

        public DocumentMetadata GetDocumentMetadata(IHtmlHelper helper, int id)
        {
            var webpage = helper.GetRequiredService<ISession>().Get<Webpage>(id);

            return GetMetadata(webpage);
        }

        private List<DocumentMetadataInfo> GetDocumentMetadataInfo()
        {
            var list = new List<DocumentMetadataInfo>();

            foreach (
                Type type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(DocumentMetadataMap<>).MakeGenericType(type));
                if (types.Any())
                {
                    var definition = _serviceProvider.GetRequiredService(types.First()) as IGetDocumentMetadataInfo;
                    list.Add(definition.Metadata);
                }
                else
                {
                    var definition =
                        _serviceProvider.GetRequiredService(typeof(DefaultDocumentMetadata<>).MakeGenericType(type)) as
                            IGetDocumentMetadataInfo;
                    list.Add(definition.Metadata);
                }
            }

            return list.OrderBy(x => x.DisplayOrder).ToList();
        }

        private DocumentMetadata GetMetadata(DocumentMetadataInfo info)
        {
            var validChildrenTypes = new List<Type>();
            switch (info.ChildrenListType)
            {
                case ChildrenListType.BlackList:
                    validChildrenTypes.AddRange(
                        DocumentMetadataInfos.Where(
                                metadata => !info.ChildrenList.Contains(metadata.Type) && !metadata.AutoBlacklist)
                            .Select(metadata => metadata.Type));
                    break;
                case ChildrenListType.WhiteList:
                    validChildrenTypes.AddRange(info.ChildrenList);
                    break;
            }

            return new DocumentMetadata(
                info.Name,
                info.IconClass,
                info.WebGetController,
                info.WebGetAction,
                info.WebPostController,
                info.WebPostAction,
                info.MaxChildNodes,
                info.Sortable,
                info.SortBy,
                info.DisplayOrder,
                info.Type,
                info.PostTypes,
                info.ChildrenListType,
                info.ChildrenList,
                info.AutoBlacklist,
                info.RequiresParent,
                info.DefaultLayoutName,
                info.EditPartialView,
                info.ShowChildrenInAdminNav,
                info.ChildrenMaintainHierarchy,
                info.HasBodyContent,
                info.RevealInNavigation,
                validChildrenTypes
            );
        }

        public Type GetTypeByName(string name)
        {
            return GetDocumentMetadatas().Where(x => x.TypeName == name).Select(x => x.Type).FirstOrDefault();
        }

        public string GetIconClass(Document document)
        {
            DocumentMetadata documentTypeDefinition =
                GetDocumentMetadatas().FirstOrDefault(
                    x => document.GetType().Name.Equals(x.TypeName, StringComparison.OrdinalIgnoreCase));

            return documentTypeDefinition.IconClass;
        }

        public DocumentMetadata GetMetadata(Type getType)
        {
            return GetDocumentMetadatas().FirstOrDefault(x => x.Type.Name == getType.Name);
        }

        public DocumentMetadata GetMetadata(Document document)
        {
            return GetDocumentMetadatas().FirstOrDefault(x => x.Type.Name == document.DocumentType);
        }

        public int? GetMaxChildNodes(Document document)
        {
            DocumentMetadata documentTypeDefinition = GetMetadata(document);
            return documentTypeDefinition.MaxChildNodes > 0
                ? documentTypeDefinition.MaxChildNodes
                : (int?) null;
        }

        public List<DocumentMetadata> GetValidParentTypes(Webpage webpage)
        {
            Type type = webpage.Unproxy().GetType();
            return GetDocumentMetadatas().FindAll(metadata => metadata.ValidChildrenTypes.Contains(type));
        }

        public DocumentMetadata GetMetadataByTypeName(string name)
        {
            return GetDocumentMetadatas().FirstOrDefault(x => x.Type.FullName == name);
        }
    }
}