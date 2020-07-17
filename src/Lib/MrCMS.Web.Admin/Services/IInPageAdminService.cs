using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IInPageAdminService
    {
        SaveResult SaveContent(UpdatePropertyData updatePropertyData);
        ContentInfo GetContent(GetPropertyData getPropertyData);
    }
}