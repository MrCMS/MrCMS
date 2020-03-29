using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class UserAvatarService : IUserAvatarService
    {
        private const string UserAvatarUrl = "user-avatar-folder";
        private readonly IFileAdminService _fileAdminService;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IUserManagementService _userManagementService;

        public UserAvatarService(IFileAdminService fileAdminService, IRepository<MediaCategory> mediaCategoryRepository, IUserManagementService userManagementService)
        {
            _fileAdminService = fileAdminService;
            _mediaCategoryRepository = mediaCategoryRepository;
            _userManagementService = userManagementService;
        }

        public async Task SetAvatar(int userId, IFormFile formFile)
        {
            var folder = await GetUserAvatarCategoryModel();
            var user = await _userManagementService.GetUser(userId);
            var extension = Path.GetExtension(formFile.FileName);
            var filename = user.Guid.ToString() + "." + extension;

            var result = await _fileAdminService.AddFile(formFile.OpenReadStream(), filename, formFile.ContentType,
                formFile.Length,
                folder);

            user.AvatarImage = result.url;
            await _userManagementService.SaveUser(user);
        }

        async Task<int> GetUserAvatarCategoryModel()
        {
            var userAvatarCategoryModel = _mediaCategoryRepository
                .Query()
                .SingleOrDefault(x => x.UrlSegment == UserAvatarUrl);
            return userAvatarCategoryModel?.Id ?? await CreateUserAvatarModel();
        }

        private async Task<int> CreateUserAvatarModel()
        {
            var avatarFolder = new MediaCategory
            {
                Name = "User Avatars",
                UrlSegment = UserAvatarUrl,
                HideInAdminNav = true
            };
            await _mediaCategoryRepository.Add(avatarFolder);
            return avatarFolder.Id;
        }
    }
}