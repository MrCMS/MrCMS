using System;
using System.Linq;
using MrCMS.Batching;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Helpers;
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

        public ImportDocumentBatchJobExecutor(ISession session,
            ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IUpdateTagsService updateTagsService, IUpdateUrlHistoryService updateUrlHistoryService)
            : base(setBatchJobJobExecutionStatus)
        {
            _session = session;
            _updateTagsService = updateTagsService;
            _updateUrlHistoryService = updateUrlHistoryService;
        }

        protected override BatchJobExecutionResult OnExecute(ImportDocumentBatchJob batchJob)
        {
            using (EventContext.Instance.Disable<IOnTransientNotificationPublished>())
            using (EventContext.Instance.Disable<IOnPersistentNotificationPublished>())
            using (EventContext.Instance.Disable<UpdateIndicesListener>())
            using (EventContext.Instance.Disable<UpdateUniversalSearch>())
            using (EventContext.Instance.Disable<WebpageUpdatedNotification>())
            using (EventContext.Instance.Disable<DocumentAddedNotification>())
            using (EventContext.Instance.Disable<MediaCategoryUpdatedNotification>())
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
                if (documentImportDto.UrlSegment != null)
                    webpage.UrlSegment = documentImportDto.UrlSegment;
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

        private Webpage GetWebpageByUrl(string urlSegment)
        {
            return _session.Query<Webpage>().FirstOrDefault(p => p.UrlSegment == urlSegment);
        }
    }
}