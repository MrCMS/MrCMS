using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Misc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class ValidWebpageChildrenService : IValidWebpageChildrenService
    {
        private readonly IExistAnyWebpageService _existAnyWebpageService;
        private readonly IDocumentMetadataService _documentMetadataService;

        public ValidWebpageChildrenService(IExistAnyWebpageService existAnyWebpageService, IDocumentMetadataService documentMetadataService)
        {
            _existAnyWebpageService = existAnyWebpageService;
            _documentMetadataService = documentMetadataService;
        }

        public IEnumerable<DocumentMetadata> GetValidWebpageDocumentTypes(Webpage webpage, Func<DocumentMetadata, bool> predicate)
        {
            var documentTypeDefinitions = new HashSet<DocumentMetadata>();
            var webpageMetadata = _documentMetadataService.WebpageMetadata.ToHashSet();
            if (webpage == null)
                documentTypeDefinitions = new HashSet<DocumentMetadata>(
                    webpageMetadata.Where(definition => !definition.RequiresParent));
            else
            {
                var documentTypeDefinition =
                    webpageMetadata.FirstOrDefault(
                        definition => definition.TypeName == webpage.Unproxy().GetType().Name);

                IEnumerable<DocumentMetadata> metadatas = documentTypeDefinition.ChildrenList.Select(_documentMetadataService.GetMetadata);
                switch (documentTypeDefinition.ChildrenListType)
                {
                    case ChildrenListType.BlackList:
                        IEnumerable<DocumentMetadata> documentMetadatas =
                            webpageMetadata.Except(metadatas).Where(def => !def.AutoBlacklist);
                        documentMetadatas.ForEach(item => documentTypeDefinitions.Add(item));
                        break;
                    case ChildrenListType.WhiteList:
                        metadatas.ForEach(metadata => documentTypeDefinitions.Add(metadata));
                        break;
                }
            }

            documentTypeDefinitions.RemoveWhere(
                definition => typeof(IUniquePage).IsAssignableFrom(definition.Type) && _existAnyWebpageService.ExistAny(definition.Type));
            if (predicate != null)
            {
                documentTypeDefinitions.RemoveWhere(metadata => !predicate(metadata));
            }

            return documentTypeDefinitions;
        }

        public bool AnyValidWebpageDocumentTypes(Webpage webpage, Func<DocumentMetadata, bool> predicate)
        {
            return GetValidWebpageDocumentTypes(webpage, predicate).Any();
        }
    }
}