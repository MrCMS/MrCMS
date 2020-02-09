using MrCMS.Batching.Entities;

namespace MrCMS.Services.WebsiteManagement
{
    public class CompleteMergeBatchJob : BatchJob
    {
        public override string Name => $"Remove the webpage with id {WebpageId} as part of a merge";

        public virtual int WebpageId { get; set; }
        public virtual int MergedIntoId { get; set; }
    }
}