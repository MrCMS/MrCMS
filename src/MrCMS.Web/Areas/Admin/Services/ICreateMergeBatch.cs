using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ICreateMergeBatch
    {
        Task<bool> CreateBatch(MergeWebpageConfirmationModel model);
    }
}