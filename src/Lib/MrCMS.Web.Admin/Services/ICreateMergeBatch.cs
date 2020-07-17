using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ICreateMergeBatch
    {
        bool CreateBatch(MergeWebpageConfirmationModel model);
    }
}