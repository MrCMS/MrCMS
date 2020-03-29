using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        Task<bool> CreateBatch(MoveWebpageConfirmationModel model);
    }
}