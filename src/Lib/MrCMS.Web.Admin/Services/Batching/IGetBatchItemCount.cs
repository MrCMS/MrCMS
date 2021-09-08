using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IGetBatchItemCount
    {
        Task<int> Get(Batch batch);
    }
}