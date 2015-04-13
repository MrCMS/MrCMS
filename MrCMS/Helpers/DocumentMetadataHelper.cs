using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;
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

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                var types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(DocumentMetadataMap<>).MakeGenericType(type));
                if (types.Any())
                {
                    var definition = MrCMSApplication.Get(types.First()) as IGetDocumentMetadata;
                    list.Add(definition.Metadata);
                }
                else
                {
                    var definition =
                        MrCMSApplication.Get(typeof(DefaultDocumentMetadata<>).MakeGenericType(type)) as
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
            DocumentMetadata documentTypeDefinition =
                DocumentMetadatas.FirstOrDefault(
                    x => document.GetType().Name.Equals(x.TypeName, StringComparison.OrdinalIgnoreCase));

            return documentTypeDefinition != null ? documentTypeDefinition.IconClass : null;
        }

        public static DocumentMetadata GetMetadata(this Type getType)
        {
            return DocumentMetadatas.FirstOrDefault(x => x.Type.Name == getType.Name);
        }

        public static DocumentMetadata GetMetadata(this Document document)
        {
            return DocumentMetadatas.FirstOrDefault(x => x.Type.Name == document.DocumentType);
        }

        public static int? GetMaxChildNodes(this Document document)
        {
            DocumentMetadata documentTypeDefinition = document.GetMetadata();
            return documentTypeDefinition != null && documentTypeDefinition.MaxChildNodes > 0
                ? documentTypeDefinition.MaxChildNodes
                : (int?)null;
        }

        public static List<DocumentMetadata> GetValidParentTypes(Webpage webpage)
        {
            Type type = webpage.Unproxy().GetType();
            return DocumentMetadatas.FindAll(metadata => metadata.ValidChildrenTypes.Contains(type));
        }

        public static DocumentMetadata GetMetadataByTypeName(string name)
        {
            return DocumentMetadatas.FirstOrDefault(x => x.Type.FullName == name);
        }
    }
}