using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching;
using MrCMS.Batching.CoreJobs;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Indexing;
using MrCMS.Services.ImportExport.BatchJobs;
using MrCMS.Services.ImportExport.DTOs;
using Newtonsoft.Json;

namespace MrCMS.Services.ImportExport
{
    public class ImportDocumentsService : IImportDocumentsService
    {
        private readonly IControlBatchRun _controlBatchRun;
        private readonly ICreateBatch _createBatch;

        public ImportDocumentsService(ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
        }

        public Batch CreateBatch(List<DocumentImportDTO> items, bool autoStart = true)
        {
            List<BatchJob> jobs = items.Select(item => new ImportDocumentBatchJob
            {
                Data = JsonConvert.SerializeObject(item),
                UrlSegment = item.UrlSegment
            } as BatchJob).ToList();
            jobs.Add(new RebuildUniversalSearchIndex());
            jobs.AddRange(IndexingHelper.IndexDefinitionTypes.Select(definition => new RebuildLuceneIndex
            {
                IndexName = definition.SystemName
            }));

            BatchCreationResult batchCreationResult = _createBatch.Create(jobs);
            if (autoStart)
                _controlBatchRun.Start(batchCreationResult.InitialBatchRun);
            return batchCreationResult.Batch;
        }
    }
}