using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class DocumentMetadataHelper
    {
        private static List<DocumentMetadata> _documentMetadatas;

        public static IEnumerable<DocumentMetadata> WebpageMetadata
        {
            get { return DocumentMetadatas.Where(x => x.Type != null && x.Type.IsSubclassOf(typeof(Webpage))); }
        }

        public static List<DocumentMetadata> DocumentMetadatas
        {
            get { return _documentMetadatas ?? (_documentMetadatas = GetDocumentMetadata()); }
        }

        private static List<DocumentMetadata> GetDocumentMetadata()
        {
            var list = new List<DocumentMetadata>();

            foreach (var type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>().Where(type => !type.ContainsGenericParameters))
            {
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(DocumentMetadataMap<>).MakeGenericType(type));
                if (types.Any())
                {
                    var definition = Activator.CreateInstance(types.First()) as IGetDocumentMetadata;
                    list.Add(definition.Metadata);
                }
                else
                {
                    var definition =
                        Activator.CreateInstance(typeof(DefaultDocumentMetadata<>).MakeGenericType(type)) as
                        IGetDocumentMetadata;
                    list.Add(definition.Metadata);
                }
            }
            return list.OrderBy(x => x.DisplayOrder).ToList();
        }

        public static Type GetTypeByName(string name)
        {
            return DocumentMetadatas.Where(x => x.TypeName == name).Select(x => x.Type).FirstOrDefault();
        }

        public static string GetIconClass(Document document)
        {
            var documentTypeDefinition =
                DocumentMetadatas.FirstOrDefault(x => document.GetType().Name.Equals(x.TypeName, StringComparison.OrdinalIgnoreCase));

            return documentTypeDefinition != null ? documentTypeDefinition.IconClass : null;
        }

        public static DocumentMetadata GetMetadataByType(Type getType)
        {
            return DocumentMetadatas.FirstOrDefault(x => x.Type.Name == getType.Name);
        }

        public static DocumentMetadata GetMetadata(this Document document)
        {
            return DocumentMetadatas.FirstOrDefault(x => x.Type.Name == document.DocumentType);
        }

        public static IEnumerable<DocumentMetadata> GetValidWebpageDocumentTypes(this Webpage parent)
        {
            var documentTypeDefinitions = new List<DocumentMetadata>();
            if (parent == null)
                documentTypeDefinitions =
                    WebpageMetadata.Where(definition => !definition.RequiresParent).ToList();
            else
            {
                var documentTypeDefinition =
                    WebpageMetadata.FirstOrDefault(
                        definition => definition.TypeName == parent.Unproxy().GetType().Name);

                if (documentTypeDefinition == null) return Enumerable.Empty<DocumentMetadata>();

                switch (documentTypeDefinition.ChildrenListType)
                {
                    case ChildrenListType.BlackList:
                        documentTypeDefinitions.AddRange(
                            WebpageMetadata.Except(
                                documentTypeDefinition.ChildrenList.Select(GetMetadataByType))
                                                          .Where(def => !def.AutoBlacklist));
                        break;
                    case ChildrenListType.WhiteList:
                        documentTypeDefinitions.AddRange(documentTypeDefinition.ChildrenList.Select(GetMetadataByType));
                        break;
                }
            }


            documentTypeDefinitions =
                documentTypeDefinitions.FindAll(
                    definition => !typeof (IUniquePage).IsAssignableFrom(definition.Type) ||
                                  (OverrideExistAny != null
                                       ? !OverrideExistAny(definition.Type)
                                       : !MrCMSApplication.Get<IDocumentService>().ExistAny(definition.Type)));

            return documentTypeDefinitions;
        }

        public static Func<Type, bool> OverrideExistAny
        {
            get { return (Func<Type, bool>) CurrentRequestData.CurrentContext.Items["current.override.existany"]; }
            set { CurrentRequestData.CurrentContext.Items["current.override.existany"] = value; }
        }

        public static int? GetMaxChildNodes(this Document document)
        {
            var documentTypeDefinition = document.GetMetadata();
            return documentTypeDefinition != null && documentTypeDefinition.MaxChildNodes > 0
                       ? documentTypeDefinition.MaxChildNodes
                       : (int?)null;
        }

        public static List<DocumentMetadata> GetValidParentTypes(Webpage webpage)
        {
            var type = webpage.Unproxy().GetType();
            return DocumentMetadatas.FindAll(metadata => metadata.ValidChildrenTypes.Contains(type));
        }
    }
}
