using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Services.ImportExport.BatchJobs;
using MrCMS.Services.ImportExport.DTOs;
using Newtonsoft.Json;

namespace MrCMS.Services.ImportExport
{
    public class ImportWebpagesService : IImportWebpagesService
    {
        private readonly IControlBatchRun _controlBatchRun;
        private readonly ICreateBatch _createBatch;

        public ImportWebpagesService(ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
        }

        public async Task<Batch> CreateBatch(List<WebpageImportDTO> items, bool autoStart = true)
        {
            List<BatchJob> jobs = items.Select(item => new ImportWebpageBatchJob
            {
                Data = JsonConvert.SerializeObject(item),
                UrlSegment = item.UrlSegment
            } as BatchJob).ToList();

            BatchCreationResult batchCreationResult = await _createBatch.Create(jobs);
            if (autoStart)
                await _controlBatchRun.Start(batchCreationResult.InitialBatchRun);
            return batchCreationResult.Batch;
        }
    }
}