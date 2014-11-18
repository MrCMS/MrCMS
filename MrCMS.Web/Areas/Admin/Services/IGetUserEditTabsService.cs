using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models.UserEdit;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetUserEditTabsService
    {
        List<UserTabBase> GetEditTabs(User user);
    }
}