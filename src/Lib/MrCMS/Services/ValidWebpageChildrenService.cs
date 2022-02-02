using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class ValidWebpageChildrenService : IValidWebpageChildrenService
    {
        private readonly IExistAnyWebpageService _existAnyWebpageService;
        private readonly IWebpageMetadataService _webpageMetadataService;

        public ValidWebpageChildrenService(IExistAnyWebpageService existAnyWebpageService,
            IWebpageMetadataService webpageMetadataService)
        {
            _existAnyWebpageService = existAnyWebpageService;
            _webpageMetadataService = webpageMetadataService;
        }

        public async Task<IReadOnlyCollection<WebpageMetadata>> GetValidWebpageDocumentTypes(Webpage webpage,
            Func<WebpageMetadata, Task<bool>> predicate)
        {
            var documentTypeDefinitions = new HashSet<WebpageMetadata>();
            var webpageMetadata = _webpageMetadataService.WebpageMetadata.ToHashSet();
            if (webpage == null)
                documentTypeDefinitions = new HashSet<WebpageMetadata>(
                    webpageMetadata.Where(definition => !definition.RequiresParent));
            else
            {
                var documentTypeDefinition =
                    webpageMetadata.FirstOrDefault(
                        definition => definition.TypeName == webpage.Unproxy().GetType().Name);

                IEnumerable<WebpageMetadata> metadatas =
                    documentTypeDefinition.ChildrenList.Select(_webpageMetadataService.GetMetadata);
                switch (documentTypeDefinition.ChildrenListType)
                {
                    case ChildrenListType.BlackList:
                        IEnumerable<WebpageMetadata> documentMetadatas =
                            webpageMetadata.Except(metadatas).Where(def => !def.AutoBlacklist);
                        documentMetadatas.ForEach(item => documentTypeDefinitions.Add(item));
                        break;
                    case ChildrenListType.WhiteList:
                        metadatas.ForEach(metadata => documentTypeDefinitions.Add(metadata));
                        break;
                }
            }

            documentTypeDefinitions.RemoveWhere(
                definition => typeof(IUniquePage).IsAssignableFrom(definition.Type) &&
                              _existAnyWebpageService.ExistAny(definition.Type));
            if (predicate != null)
            {
                foreach (var documentMetadata in documentTypeDefinitions.ToList()) // copy
                {
                    if (!await predicate(documentMetadata))
                    {
                        documentTypeDefinitions.Remove(documentMetadata);
                    }
                }
            }

            return documentTypeDefinitions;
        }

        public async Task<bool> AnyValidWebpageDocumentTypes(Webpage webpage, Func<WebpageMetadata, Task<bool>> predicate)
        {
            return (await GetValidWebpageDocumentTypes(webpage, predicate)).Any();
        }
    }
}