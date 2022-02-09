using System.Collections.Generic;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IGetAclOptions
    {
        List<AclInfo> GetInfos();
    }
}