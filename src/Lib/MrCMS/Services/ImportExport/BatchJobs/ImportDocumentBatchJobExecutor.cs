using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MrCMS.Batching;
using MrCMS.Data;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Search;
using MrCMS.Services.Notifications;

namespace MrCMS.Services.ImportExport.BatchJobs
{
    public class ImportDocumentBatchJobExecutor : BaseBatchJobExecutor<ImportDocumentBatchJob>
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IUpdateTagsService _updateTagsService;
        private readonly IUpdateUrlHistoryService _updateUrlHistoryService;
        private readonly IEventContext _eventContext;
        private readonly IWebpageUrlService _webpageUrlService;


        public ImportDocumentBatchJobExecutor(
            IRepository<Webpage> repository,
            IUpdateTagsService updateTagsService, IUpdateUrlHistoryService updateUrlHistoryService,
            IEventContext eventContext,
            IWebpageUrlService webpageUrlService)
        {
            _repository = repository;
            _updateTagsService = updateTagsService;
            _updateUrlHistoryService = updateUrlHistoryService;
            _eventContext = eventContext;
            _webpageUrlService = webpageUrlService;
        }

        [ItemCanBeNull]
        protected async override Task<BatchJobExecutionResult> OnExecuteAsync(ImportDocumentBatchJob batchJob,
            CancellationToken token)
        {
            using (_eventContext.Disable<IOnTransientNotificationPublished>())
            using (_eventContext.Disable<IOnPersistentNotificationPublished>())
            //using (_eventContext.Disable<UpdateIndicesListener>())
            using (_eventContext.Disable<UpdateUniversalSearch>())
            using (_eventContext.Disable<WebpageUpdatedNotification>())
            using (_eventContext.Disable<DocumentAddedNotification>())
            using (_eventContext.Disable<MediaCategoryUpdatedNotification>())
            {
                var documentImportDto = batchJob.DocumentImportDto;
                var webpage =
                    await GetWebpageByUrl(documentImportDto.UrlSegment);

                var isNew = webpage == null;
                if (isNew)
                    webpage = (Webpage)
                        Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(documentImportDto.DocumentType));

                if (!String.IsNullOrEmpty(documentImportDto.ParentUrl))
                {
                    var parent = await GetWebpageByUrl(documentImportDto.ParentUrl);
                    webpage.Parent = parent;
                }

                if (!string.IsNullOrWhiteSpace(documentImportDto.UrlSegment) && isNew)
                {
                    webpage.UrlSegment = await _webpageUrlService.Suggest(new SuggestParams
                    {
                        DocumentType = documentImportDto.DocumentType,
                        ParentId = webpage.Parent?.Id,
                        PageName = documentImportDto.Name
                    });
                }
                webpage.Name = documentImportDto.Name;
                webpage.BodyContent = documentImportDto.BodyContent;
                webpage.MetaTitle = documentImportDto.MetaTitle;
                webpage.MetaDescription = documentImportDto.MetaDescription;
                webpage.MetaKeywords = documentImportDto.MetaKeywords;
                webpage.RevealInNavigation = documentImportDto.RevealInNavigation;
                webpage.RequiresSSL = documentImportDto.RequireSSL;
                webpage.DisplayOrder = documentImportDto.DisplayOrder;
                webpage.PublishOn = documentImportDto.PublishDate;

                _updateTagsService.SetTags(documentImportDto, webpage);
                _updateUrlHistoryService.SetUrlHistory(documentImportDto, webpage);

                await _repository.Transact(async (repo, ct) =>
                {
                    if (isNew)
                        await repo.Add(webpage, token);
                    else
                        await repo.Update(webpage, token);
                }, token);

                return BatchJobExecutionResult.Success();
            }
        }

        private Task<Webpage> GetWebpageByUrl(string urlSegment)
        {
            return _repository.Query().FirstOrDefaultAsync(p => p.UrlSegment == urlSegment);
        }
    }
}