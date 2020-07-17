using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IAclAdminService
    {
        List<AclInfo> GetOptions();
        bool UpdateAcl(IFormCollection collection);
    }
}