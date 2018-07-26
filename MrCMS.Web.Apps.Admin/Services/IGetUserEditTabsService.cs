using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Models.UserEdit;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetUserEditTabsService
    {
        List<UserTabBase> GetEditTabs(User user);
    }
}