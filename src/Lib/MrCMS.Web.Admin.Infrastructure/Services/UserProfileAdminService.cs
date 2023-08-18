using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Mapping;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.Models;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public class UserProfileAdminService : IUserProfileAdminService
    {
        private readonly ISessionAwareMapper _mapper;
        private readonly IUserProfileDataService _userProfileDataService;

        public UserProfileAdminService(ISessionAwareMapper mapper, IUserProfileDataService userProfileDataService)
        {
            _mapper = mapper;
            _userProfileDataService = userProfileDataService;
        }

        public async Task<T> Add<T, TModel>(TModel model)
            where T : UserProfileData where TModel : IAddUserProfileDataModel
        {
            var profileData = _mapper.Map<T>(model);

            await _userProfileDataService.Add(profileData);

            return profileData;
        }

        public async Task<T> Update<T, TModel>(TModel model) where T : UserProfileData where TModel : IHaveId
        {
            var profileData = await _userProfileDataService.Get<T>(model.Id.GetValueOrDefault());
            if (profileData == null)
            {
                return null;
            }

            _mapper.Map(model, profileData);
            await _userProfileDataService.Update(profileData);

            return profileData;
        }

        public async Task Delete<T>(int id) where T : UserProfileData
        {
            var profileData =await _userProfileDataService.Get<T>(id);
            if (profileData == null)
            {
                return;
            }

            await _userProfileDataService.Delete(profileData);
        }

        public async Task<TModel> GetEditModel<T, TModel>(int id) where T : UserProfileData where TModel : IHaveId
        {
            var profileData = await _userProfileDataService.Get<T>(id);
            return _mapper.Map<TModel>(profileData);
        }
    }
}