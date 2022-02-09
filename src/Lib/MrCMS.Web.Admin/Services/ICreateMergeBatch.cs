using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ICreateMergeBatch
    {
        Task<bool> CreateBatch(MergeWebpageConfirmationModel model);
    }
}