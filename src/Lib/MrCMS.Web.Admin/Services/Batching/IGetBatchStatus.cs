using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IGetBatchStatus
    {
        Task<BatchStatus> Get(Batch batch);
    }
}