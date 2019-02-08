using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Infrastructure.Models;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Services
{
    public interface IUserProfileAdminService
    {
        T Add<T, TModel>(TModel model) where T : UserProfileData where TModel : IAddUserProfileDataModel;
        T Update<T, TModel>(TModel model) where T : UserProfileData where TModel : IHaveId;
        void Delete<T>(int id) where T : UserProfileData;
        TModel GetEditModel<T, TModel>(int id) where T : UserProfileData where TModel : IHaveId;
    }
}
