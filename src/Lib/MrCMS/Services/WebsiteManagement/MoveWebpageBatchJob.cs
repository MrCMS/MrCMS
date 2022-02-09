using MrCMS.Batching.Entities;

namespace MrCMS.Services.WebsiteManagement
{
    public class MoveWebpageBatchJob : BatchJob
    {
        public override string Name => $"Move the webpage with id {WebpageId} as part of a merge";

        public virtual int WebpageId { get; set; }
        public virtual int? NewParentId { get; set; }
    }
}