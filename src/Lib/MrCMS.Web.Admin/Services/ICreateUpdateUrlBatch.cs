using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        Task<bool> CreateBatch(MoveWebpageConfirmationModel model);
    }
}