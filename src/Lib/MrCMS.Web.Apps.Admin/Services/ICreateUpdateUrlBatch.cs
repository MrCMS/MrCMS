using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        Task<bool> CreateBatch(MoveWebpageConfirmationModel model);
    }
}