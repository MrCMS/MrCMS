using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        bool CreateBatch(MoveWebpageConfirmationModel model);
    }
}