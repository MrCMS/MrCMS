using MrCMS.Batching.Entities;
using MrCMS.Services.ImportExport.DTOs;
using Newtonsoft.Json;

namespace MrCMS.Services.ImportExport.BatchJobs
{
    public class ImportWebpageBatchJob : BatchJob
    {
        public virtual string UrlSegment { get; set; }
        public virtual WebpageImportDTO WebpageImportDto
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<WebpageImportDTO>(Data);
                }
                catch
                {
                    return null;
                }
            }
        }

        public override string Name => "Import Document - " + UrlSegment;
    }
}