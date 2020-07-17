using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        bool CreateBatch(MoveWebpageConfirmationModel model);
    }
}