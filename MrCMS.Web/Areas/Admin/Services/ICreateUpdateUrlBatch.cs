using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ICreateUpdateUrlBatch
    {
        bool CreateBatch(MoveWebpageConfirmationModel model);
    }
}