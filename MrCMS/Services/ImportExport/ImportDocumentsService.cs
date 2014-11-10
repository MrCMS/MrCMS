using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.CoreJobs;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing;
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
        private readonly IControlBatchRun _controlBatchRun;

        public ImportDocumentsService(ISession session, ICreateBatchRun createBatchRun,IControlBatchRun controlBatchRun)
        {
            _session = session;
            _createBatchRun = createBatchRun;
            _controlBatchRun = controlBatchRun;
        }

        public void CreateBatch(List<DocumentImportDTO> items)
        {
            var batch = new Batch { BatchJobs = new List<BatchJob>() };
            _session.Transact(session => session.Save(batch));
            _session.Transact(session =>
            {
                foreach (var item in items)
                {
                    var importDocumentBatchJob = new ImportDocumentBatchJob
                    {
                        Batch = batch,
                        Data = JsonConvert.SerializeObject(item),
                        UrlSegment = item.UrlSegment
                    };
                    batch.BatchJobs.Add(importDocumentBatchJob);
                    session.Save(importDocumentBatchJob);
                }
                // Reindex Universal search when done
                var universalIndexRebuilder = new RebuildUniversalSearchIndex
                {
                    Batch = batch
                };
                batch.BatchJobs.Add(universalIndexRebuilder);
                session.Save(universalIndexRebuilder);

                // Reindex standard indexes
                foreach (var type in IndexingHelper.IndexDefinitionTypes)
                {
                    var luceneIndex = new RebuildLuceneIndex
                    {
                        Batch = batch,
                        IndexName = type.SystemName
                    };
                    batch.BatchJobs.Add(luceneIndex);
                    session.Save(luceneIndex);
                }
            });
            var batchRun = _createBatchRun.Create(batch);
            _controlBatchRun.Start(batchRun);
        }
    }
}