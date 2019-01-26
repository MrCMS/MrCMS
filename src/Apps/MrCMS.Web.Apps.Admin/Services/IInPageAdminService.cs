using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IInPageAdminService
    {
        SaveResult SaveBodyContent(UpdatePropertyData updatePropertyData);
        string GetUnformattedBodyContent(GetPropertyData getPropertyData);
        string GetFormattedBodyContent(GetPropertyData getPropertyData, Controller controller);
    }
}