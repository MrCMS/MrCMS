using System.Threading.Tasks;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class GetNotificationModifiedUserInfo : IGetNotificationModifiedUserInfo
    {
        private readonly IGetCurrentUser _currentUser;

        public GetNotificationModifiedUserInfo(IGetCurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<string> GetInfo()
        {
            var user = await _currentUser.Get();
            return user != null
                ? string.Format(" by <a href=\"/Admin/User/Edit/{1}\">{0}</a>", user.Name, user.Id)
                : string.Empty;
        }
    }
}