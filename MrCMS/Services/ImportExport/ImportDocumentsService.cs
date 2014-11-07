using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.BatchJobs;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Services.Notifications;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Services.ImportExport
{
    public class ImportDocumentsService : IImportDocumentsService
    {
        private readonly ISession _session;
        private readonly ICreateBatchRun _createBatchRun;
        //private readonly Site _site;
        //private readonly IIndexService _indexService;
        //private readonly IUpdateTagsService _updateTagsService;
        //private readonly IUpdateUrlHistoryService _updateUrlHistoryService;

        public ImportDocumentsService(ISession session, ICreateBatchRun createBatchRun
            //Site site, IIndexService indexService, IUpdateTagsService updateTagsService, IUpdateUrlHistoryService updateUrlHistoryService
            )
        {
            _session = session;
            _createBatchRun = createBatchRun;
            //_site = site;
            //_indexService = indexService;
            //_updateTagsService = updateTagsService;
            //_updateUrlHistoryService = updateUrlHistoryService;
        }

        ///// <summary>
        ///// Import All from DTOs
        ///// </summary>
        ///// <param name="items"></param>
        //public void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDTO> items)
        //{
        //    var eventContext = EventContext.Instance;
        //    using (eventContext.Disable(typeof(IOnPersistentNotificationPublished), typeof(IOnTransientNotificationPublished)))
        //    {
        //        var dataTransferObjects = new HashSet<DocumentImportDTO>(items);
        //        var webpages =
        //            new HashSet<Webpage>(_session.QueryOver<Webpage>().Where(webpage => webpage.Site == _site).List());
        //        _updateTagsService.Inititalise();
        //        _updateUrlHistoryService.Initialise();

        //        _session.Transact(session =>
        //        {
        //            foreach (
        //                var dataTransferObject in
        //                    dataTransferObjects.OrderBy(o => GetHierarchyDepth(o, dataTransferObjects))
        //                        .ThenBy(o => GetRootParentUrl(o, dataTransferObjects)))
        //                ImportDocument(dataTransferObject);

        //            _updateTagsService.SaveTags();
        //            _updateUrlHistoryService.SaveUrlHistories();
        //            webpages.ForEach(session.SaveOrUpdate);
        //        });
        //    }
        //    _indexService.InitializeAllIndices();
        //}

        //public static int GetHierarchyDepth(DocumentImportDTO dto, HashSet<DocumentImportDTO> allItems)
        //{
        //    var currentDto = dto;
        //    int depth = 0;
        //    while (currentDto != null && !string.IsNullOrWhiteSpace(currentDto.ParentUrl))
        //    {
        //        DocumentImportDTO dto1 = currentDto;
        //        currentDto = allItems.FirstOrDefault(o => o.UrlSegment == dto1.ParentUrl);
        //        depth++;
        //    }
        //    return depth;
        //}

        //public static string GetRootParentUrl(DocumentImportDTO dto, HashSet<DocumentImportDTO> allItems)
        //{
        //    var currentDto = dto;
        //    while (currentDto != null && !string.IsNullOrWhiteSpace(currentDto.ParentUrl))
        //    {
        //        DocumentImportDTO dto1 = currentDto;
        //        currentDto = allItems.FirstOrDefault(o => o.UrlSegment == dto1.ParentUrl);
        //    }
        //    if (currentDto != null) return currentDto.UrlSegment;
        //    return "";
        //}

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dto"></param>
        //public Webpage ImportDocument(DocumentImportDTO dto)
        //{
        //    var documentByUrl = _webpages.SingleOrDefault(x => x.UrlSegment == dto.UrlSegment);
        //    var webpage = documentByUrl ??
        //                   (Webpage)
        //                   Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(dto.DocumentType));

        //    if (!String.IsNullOrEmpty(dto.ParentUrl))
        //    {
        //        var parent = _webpages.SingleOrDefault(x => x.UrlSegment == dto.ParentUrl);
        //        webpage.Parent = parent;
        //    }
        //    if (dto.UrlSegment != null)
        //        webpage.UrlSegment = dto.UrlSegment;
        //    webpage.Name = dto.Name;
        //    webpage.BodyContent = dto.BodyContent;
        //    webpage.MetaTitle = dto.MetaTitle;
        //    webpage.MetaDescription = dto.MetaDescription;
        //    webpage.MetaKeywords = dto.MetaKeywords;
        //    webpage.RevealInNavigation = dto.RevealInNavigation;
        //    webpage.RequiresSSL = dto.RequireSSL;
        //    webpage.DisplayOrder = dto.DisplayOrder;
        //    webpage.PublishOn = dto.PublishDate;

        //    _updateTagsService.SetTags(dto, webpage);
        //    //Url History
        //    _updateUrlHistoryService.SetUrlHistory(dto, webpage);

        //    if (!_webpages.Contains(webpage))
        //        _webpages.Add(webpage);

        //    return webpage;
        //}

        public void CreateBatch(List<DocumentImportDTO> items)
        {
            var batch = new Batch { BatchJobs = new List<BatchJob>() };
            _session.Transact(session => session.Save(batch));
            foreach (var item in items)
            {
                var importDocumentBatchJob = new ImportDocumentBatchJob
                {
                    Batch = batch,
                    Data = JsonConvert.SerializeObject(item),
                    UrlSegment = item.UrlSegment
                };
                batch.BatchJobs.Add(importDocumentBatchJob);
                _session.Transact(session => session.Save(importDocumentBatchJob));
            }
            _createBatchRun.Create(batch);
        }
    }
}