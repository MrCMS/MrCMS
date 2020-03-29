using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAclAdminService
    {
        List<AclInfo> GetOptions();
        Task<bool> UpdateAcl(IFormCollection collection);
    }
}