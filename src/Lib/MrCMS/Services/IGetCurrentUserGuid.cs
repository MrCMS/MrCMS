using System;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface IGetCurrentUserGuid
    {
        Task<Guid> Get();
        void ClearCookies();
    }
}