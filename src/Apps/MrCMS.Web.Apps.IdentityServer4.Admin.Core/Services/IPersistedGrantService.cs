using System.Threading.Tasks;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Grant;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services
{
    public interface IPersistedGrantService
    {

        Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10);
        Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10);
        Task<PersistedGrantDto> GetPersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantsAsync(string userId);
    }
}
