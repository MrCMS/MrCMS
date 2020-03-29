using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetAclOptions
    {
        List<AclInfo> GetInfos();
    }
}