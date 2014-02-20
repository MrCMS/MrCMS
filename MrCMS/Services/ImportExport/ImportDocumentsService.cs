using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate;

namespace MrCMS.Services.ImportExport
{
    public class ImportDocumentsService : IImportDocumentsService
    {
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IIndexService _indexService;
        private readonly IUpdateTagsService _updateTagsService;
        private readonly IUpdateUrlHistoryService _updateUrlHistoryService;
        private HashSet<Webpage> _webpages = new HashSet<Webpage>();
        private HashSet<UrlHistory> _urlHistories = new HashSet<UrlHistory>();

        public ImportDocumentsService(ISession session, Site site, IIndexService indexService, IUpdateTagsService updateTagsService, IUpdateUrlHistoryService updateUrlHistoryService)
        {
            _session = session;
            _site = site;
            _indexService = indexService;
            _updateTagsService = updateTagsService;
            _updateUrlHistoryService = updateUrlHistoryService;
        }

        /// <summary>
        /// Import All from DTOs
        /// </summary>
        /// <param name="items"></param>
        public void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDTO> items)
        {
            var dataTransferObjects = new HashSet<DocumentImportDTO>(items);
            _webpages = new HashSet<Webpage>(_session.QueryOver<Webpage>().Where(webpage => webpage.Site == _site).List());
            _updateTagsService.Inititalise();
            _updateUrlHistoryService.Initialise();
            _urlHistories =
                new HashSet<UrlHistory>(_session.QueryOver<UrlHistory>().Where(tag => tag.Site == _site).List());

            _session.Transact(session =>
            {
                foreach (var dataTransferObject in dataTransferObjects.OrderBy(o => GetHierarchyDepth(o, dataTransferObjects)).ThenBy(o => GetRootParentUrl(o,dataTransferObjects)))
                    ImportDocument(dataTransferObject);

                _updateTagsService.SaveTags();
                _updateUrlHistoryService.SaveUrlHistories();
                _webpages.ForEach(session.SaveOrUpdate);
            });
            _indexService.InitializeAllIndices();
        }

        public static int GetHierarchyDepth(DocumentImportDTO dto, HashSet<DocumentImportDTO> allItems)
        {
            var currentDto = dto;
            int depth = 0;
            while (!string.IsNullOrWhiteSpace(currentDto.ParentUrl))
            {
                currentDto = allItems.First(o => o.UrlSegment == currentDto.ParentUrl);
                depth++;
            }
            return depth;
        }

        public static string GetRootParentUrl(DocumentImportDTO dto, HashSet<DocumentImportDTO> allItems)
        {
            var currentDto = dto;
            while (!string.IsNullOrWhiteSpace(currentDto.ParentUrl))
            {
                currentDto = allItems.First(o => o.UrlSegment == currentDto.ParentUrl);
            }
            return currentDto.UrlSegment;
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dto"></param>
        public Webpage ImportDocument(DocumentImportDTO dto)
        {
            var documentByUrl = _webpages.SingleOrDefault(x => x.UrlSegment == dto.UrlSegment);
            var webpage = documentByUrl ??
                           (Webpage)
                           Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(dto.DocumentType));

            if (!String.IsNullOrEmpty(dto.ParentUrl))
            {
                var parent = _webpages.SingleOrDefault(x => x.UrlSegment == dto.ParentUrl);
                webpage.SetParent(parent);
            }
            if (dto.UrlSegment != null)
                webpage.UrlSegment = dto.UrlSegment;
            webpage.Name = dto.Name;
            webpage.BodyContent = dto.BodyContent;
            webpage.MetaTitle = dto.MetaTitle;
            webpage.MetaDescription = dto.MetaDescription;
            webpage.MetaKeywords = dto.MetaKeywords;
            webpage.RevealInNavigation = dto.RevealInNavigation;
            webpage.RequiresSSL = dto.RequireSSL;
            webpage.DisplayOrder = dto.DisplayOrder;
            webpage.PublishOn = dto.PublishDate;

            _updateTagsService.SetTags(dto, webpage);
            //Url History
         _updateUrlHistoryService.SetUrlHistory(dto, webpage);

            if (!_webpages.Contains(webpage))
                _webpages.Add(webpage);

            return webpage;
        }

        private void SetUrlHistory(DocumentImportDTO documentDto, Webpage webpage)
        {
            foreach (var item in documentDto.UrlHistory)
            {
                if (!String.IsNullOrWhiteSpace(item) && webpage.Urls.All(x => x.UrlSegment != item))
                {
                    if (_urlHistories.FirstOrDefault(history => history.UrlSegment == item) == null)
                    {
                        var urlHistory = new UrlHistory {UrlSegment = item, Webpage = webpage};
                        webpage.Urls.Add(urlHistory);
                        _urlHistories.Add(urlHistory);
                    }
                }
            }
        }
    }
}