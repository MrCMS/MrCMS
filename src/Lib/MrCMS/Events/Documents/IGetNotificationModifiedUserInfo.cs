using System.Threading.Tasks;

namespace MrCMS.Events.Documents
{
    public interface IGetNotificationModifiedUserInfo
    {
        Task<string> GetInfo();
    }
}