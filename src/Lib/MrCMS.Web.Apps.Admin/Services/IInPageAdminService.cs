using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IInPageAdminService
    {
        SaveResult SaveContent(UpdatePropertyData updatePropertyData);
        ContentInfo GetContent(GetPropertyData getPropertyData);
    }
}