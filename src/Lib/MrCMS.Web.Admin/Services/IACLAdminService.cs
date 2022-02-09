using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IAclAdminService
    {
        Task<List<AclInfo>> GetOptions();
        Task<bool> UpdateAcl(IFormCollection collection);
    }
}