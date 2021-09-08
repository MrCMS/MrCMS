using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Admin.Services
{
    public class UserAvatarService : IUserAvatarService
    {
        private const string UserAvatarUrl = "user-avatar-folder";
        private readonly IFileAdminService _fileAdminService;
        private readonly ISession _session;

        public UserAvatarService(IFileAdminService fileAdminService, ISession session)
        {
            _fileAdminService = fileAdminService;
            _session = session;
        }

        public async Task SetAvatar(int userId, IFormFile formFile)
        {
            var folder = await GetUserAvatarCategoryModel();
            var user = _session.Get<User>(userId);
            var extension = Path.GetExtension(formFile.FileName);
            var filename = user.Guid.ToString() + "." + extension;

            var result = await _fileAdminService.AddFile(formFile.OpenReadStream(), filename, formFile.ContentType,
                formFile.Length,
                folder);

            user.AvatarImage = (await _session.GetAsync<MediaFile>(result.Id))?.FileUrl;
            await _session.TransactAsync(x => x.UpdateAsync(user));
        }

        async Task<int> GetUserAvatarCategoryModel()
        {
            var userAvatarCategoryModel = await _session.QueryOver<MediaCategory>()
                .Where(x => x.UrlSegment == UserAvatarUrl)
                .SingleOrDefaultAsync();
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
            await _session.TransactAsync(x => x.SaveAsync(avatarFolder));
            return avatarFolder.Id;
        }
    }
}