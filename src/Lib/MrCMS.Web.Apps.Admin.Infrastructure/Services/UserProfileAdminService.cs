using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Infrastructure.Models;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Services
{
    public class UserProfileAdminService : IUserProfileAdminService
    {
        private readonly IMapper _mapper;
        private readonly IUserProfileDataService _userProfileDataService;

        public UserProfileAdminService(IMapper mapper, IUserProfileDataService userProfileDataService)
        {
            _mapper = mapper;
            _userProfileDataService = userProfileDataService;
        }
        public T Add<T, TModel>(TModel model) where T : UserProfileData where TModel : IAddUserProfileDataModel
        {
            var profileData = _mapper.Map<T>(model);

            _userProfileDataService.Add(profileData);

            return profileData;
        }

        public T Update<T, TModel>(TModel model) where T : UserProfileData where TModel : IHaveId
        {
            var profileData = _userProfileDataService.Get<T>(model.Id.GetValueOrDefault());
            if (profileData == null)
            {
                return null;
            }

            _mapper.Map(model, profileData);
            _userProfileDataService.Update(profileData);

            return profileData;
        }

        public void Delete<T>(int id) where T : UserProfileData
        {
            var profileData = _userProfileDataService.Get<T>(id);
            if (profileData == null)
            {
                return;
            }

            _userProfileDataService.Delete(profileData);
        }

        public TModel GetEditModel<T, TModel>(int id) where T : UserProfileData where TModel : IHaveId
        {
            var profileData = _userProfileDataService.Get<T>(id);
            return _mapper.Map<TModel>(profileData);
        }
    }
}