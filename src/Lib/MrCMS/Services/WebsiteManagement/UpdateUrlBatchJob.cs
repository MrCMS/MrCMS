using MrCMS.Batching.Entities;

namespace MrCMS.Services.WebsiteManagement
{
    public class UpdateUrlBatchJob : BatchJob
    {
        public override string Name => $"Update the url of page id {WebpageId} to /{NewUrl}";

        public virtual int WebpageId { get; set; }
        public virtual string NewUrl { get; set; }
    }
}