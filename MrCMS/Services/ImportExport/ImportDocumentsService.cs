using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents;
using NHibernate;

namespace MrCMS.Services.ImportExport
{
    public class ImportDocumentsService : IImportDocumentsService
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly IUrlHistoryService _urlHistoryService;
        private readonly ISession _session;
        private List<Document> _allDocuments;

        public ImportDocumentsService(IDocumentService documentService, ITagService tagService, IUrlHistoryService urlHistoryService, ISession session)
        {
            _session = session;
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
            _allDocuments = _documentService.GetAllDocuments<Document>().ToList();

            _session.Transact(session =>
            {
                foreach (var dataTransferObject in items)
                {
                    var transferObject = dataTransferObject;
                    ImportDocument(transferObject);
                }
                _allDocuments.ForEach(session.SaveOrUpdate);
            });
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public Webpage ImportDocument(DocumentImportDataTransferObject dataTransferObject)
        {
            if (_allDocuments == null)
                _allDocuments = new List<Document>();

            var documentByUrl = _allDocuments.OfType<Webpage>().SingleOrDefault(x => x.UrlSegment == dataTransferObject.UrlSegment);
            var document = documentByUrl ??(Webpage)Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(dataTransferObject.DocumentType));

            if (!String.IsNullOrEmpty(dataTransferObject.ParentUrl))
            {
                var parent = _allDocuments.OfType<Webpage>().SingleOrDefault(x => x.UrlSegment == dataTransferObject.ParentUrl);
                document.Parent = parent;
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
            {
                document.DisplayOrder = _allDocuments.Count();
                _allDocuments.Add(document);
            }

            return document;
        }
    }
}