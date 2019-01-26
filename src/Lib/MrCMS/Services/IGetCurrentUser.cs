using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IGetCurrentUser
    {
        User Get();
    }
}