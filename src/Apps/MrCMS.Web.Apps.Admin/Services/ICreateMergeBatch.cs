using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ICreateMergeBatch
    {
        bool CreateBatch(MergeWebpageConfirmationModel model);
    }
}