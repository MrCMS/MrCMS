using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Models;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public interface IUserProfileAdminService
    {
        Task<T> Add<T, TModel>(TModel model) where T : UserProfileData where TModel : IAddUserProfileDataModel;
        Task<T> Update<T, TModel>(TModel model) where T : UserProfileData where TModel : IHaveId;
        Task Delete<T>(int id) where T : UserProfileData;
        Task<TModel> GetEditModel<T, TModel>(int id) where T : UserProfileData where TModel : IHaveId;
    }
}
