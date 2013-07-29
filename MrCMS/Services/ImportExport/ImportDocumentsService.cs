using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents;

namespace MrCMS.Services.ImportExport
{
    public class ImportDocumentsService : IImportDocumentsService
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly IUrlHistoryService _urlHistoryService;

        public ImportDocumentsService(IDocumentService documentService, ITagService tagService, IUrlHistoryService urlHistoryService)
        {
            _documentService = documentService;
            _tagService = tagService;
            _urlHistoryService = urlHistoryService;
        }

        /// <summary>
        /// Import All from DTOs
        /// </summary>
        /// <param name="items"></param>
        public void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDataTransferObject> items)
        {
            foreach (var dataTransferObject in items)
            {
                ImportDocument(dataTransferObject);
            }
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public Webpage ImportDocument(DocumentImportDataTransferObject dataTransferObject)
        {
            var documentByUrl = _documentService.GetDocumentByUrl<Webpage>(dataTransferObject.UrlSegment);
            var document = documentByUrl ??
                           (Webpage)
                           Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(dataTransferObject.DocumentType));
            
            if (!String.IsNullOrEmpty(dataTransferObject.ParentUrl))
            {
                var parent = _documentService.GetDocumentByUrl<Webpage>(dataTransferObject.ParentUrl);
                document.SetParent(parent);
            }
            if (dataTransferObject.UrlSegment != null)
                document.UrlSegment = dataTransferObject.UrlSegment;
            document.Name = dataTransferObject.Name;
            document.BodyContent = dataTransferObject.BodyContent;
            document.MetaTitle = dataTransferObject.MetaTitle;
            document.MetaDescription = dataTransferObject.MetaDescription;
            document.MetaKeywords = dataTransferObject.MetaKeywords;
            document.RevealInNavigation = dataTransferObject.RevealInNavigation;
            document.RequiresSSL = dataTransferObject.RequireSSL;
            document.DisplayOrder = dataTransferObject.DisplayOrder;
            if (dataTransferObject.PublishDate != null)
                document.PublishOn = dataTransferObject.PublishDate;
            
            //Tags
            foreach (var item in dataTransferObject.Tags)
            {
                var tag = _tagService.GetByName(item);
                if (tag == null)
                {
                    tag = new Tag { Name = item };
                    _tagService.Add(tag);
                }
                if (!document.Tags.Contains(tag))
                    document.Tags.Add(tag);
            }
            //Url History
            foreach (var item in dataTransferObject.UrlHistory)
            {
                if (!String.IsNullOrWhiteSpace(item) && document.Urls.All(x => x.UrlSegment != item))
                {
                    if (_urlHistoryService.GetByUrlSegment(item) == null)
                        _urlHistoryService.Add(new UrlHistory { UrlSegment = item, Webpage = document });
                }
            }

            if (document.Id == 0)
                _documentService.AddDocument(document);
            else
                _documentService.SaveDocument(document);


            return document;
        }
    }
}