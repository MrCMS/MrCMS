using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IAclAdminService
    {
        List<AclInfo> GetOptions();
        Task<bool> UpdateAcl(IFormCollection collection);
    }
}