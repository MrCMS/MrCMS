using System.Collections.Generic;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetAclOptions
    {
        List<AclInfo> GetInfos();
    }
}