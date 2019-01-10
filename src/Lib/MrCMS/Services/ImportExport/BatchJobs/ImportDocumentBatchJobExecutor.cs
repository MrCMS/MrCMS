using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Search;
using MrCMS.Services.Notifications;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport.BatchJobs
{
    public class ImportDocumentBatchJobExecutor : BaseBatchJobExecutor<ImportDocumentBatchJob>
    {
        private readonly ISession _session;
        private readonly IUpdateTagsService _updateTagsService;
        private readonly IUpdateUrlHistoryService _updateUrlHistoryService;
        private readonly IEventContext _eventContext;
        private readonly IWebpageUrlService _webpageUrlService;


        public ImportDocumentBatchJobExecutor(ISession session,
            IUpdateTagsService updateTagsService, IUpdateUrlHistoryService updateUrlHistoryService,
            IEventContext eventContext,
            IWebpageUrlService webpageUrlService)
        {
            _session = session;
            _updateTagsService = updateTagsService;
            _updateUrlHistoryService = updateUrlHistoryService;
            _eventContext = eventContext;
            _webpageUrlService = webpageUrlService;
        }

        protected override BatchJobExecutionResult OnExecute(ImportDocumentBatchJob batchJob)
        {
            using (_eventContext.Disable<IOnTransientNotificationPublished>())
            using (_eventContext.Disable<IOnPersistentNotificationPublished>())
            using (_eventContext.Disable<UpdateIndicesListener>())
            using (_eventContext.Disable<UpdateUniversalSearch>())
            using (_eventContext.Disable<WebpageUpdatedNotification>())
            using (_eventContext.Disable<DocumentAddedNotification>())
            using (_eventContext.Disable<MediaCategoryUpdatedNotification>())
            {
                var documentImportDto = batchJob.DocumentImportDto;
                var webpage =
                    GetWebpageByUrl(documentImportDto.UrlSegment);

                var isNew = webpage == null;
                if (isNew)
                    webpage = (Webpage)
                        Activator.CreateInstance(DocumentMetadataHelper.GetTypeByName(documentImportDto.DocumentType));

                if (!String.IsNullOrEmpty(documentImportDto.ParentUrl))
                {
                    var parent = GetWebpageByUrl(documentImportDto.ParentUrl);
                    webpage.Parent = parent;
                }

                if (!string.IsNullOrWhiteSpace(documentImportDto.UrlSegment) && isNew)
                {
                    webpage.UrlSegment = _webpageUrlService.Suggest(new SuggestParams
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

                _session.Transact(session =>
                {
                    if (isNew)
                        session.Save(webpage);
                    else
                        session.Update(webpage);
                });

                return BatchJobExecutionResult.Success();
            }
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(ImportDocumentBatchJob batchJob)
        {
            throw new NotImplementedException();
        }

        private Webpage GetWebpageByUrl(string urlSegment)
        {
            return _session.Query<Webpage>().FirstOrDefault(p => p.UrlSegment == urlSegment);
        }
    }
}