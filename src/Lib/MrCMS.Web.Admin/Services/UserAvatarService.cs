using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
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

        public void SetAvatar(int userId, IFormFile formFile)
        {
            var folder = GetUserAvatarCategoryModel();
            var user = _session.Get<User>(userId);
            var extension = Path.GetExtension(formFile.FileName);
            var filename = user.Guid.ToString() + "." + extension;

            var result = _fileAdminService.AddFile(formFile.OpenReadStream(), filename, formFile.ContentType,
                formFile.Length,
                folder);

            user.AvatarImage = _session.Get<MediaFile>(result.Id)?.FileUrl;
            _session.Transact(x => x.Update(user));
        }

        int GetUserAvatarCategoryModel()
        {
            var userAvatarCategoryModel = _session.QueryOver<MediaCategory>()
                .Where(x => x.UrlSegment == UserAvatarUrl)
                .SingleOrDefault();
            return userAvatarCategoryModel?.Id ?? CreateUserAvatarModel();
        }

        private int CreateUserAvatarModel()
        {
            var avatarFolder = new MediaCategory
            {
                Name = "User Avatars",
                UrlSegment = UserAvatarUrl,
                HideInAdminNav = true
            };
            _session.Transact(x => x.Save(avatarFolder));
            return avatarFolder.Id;
        }
    }
}