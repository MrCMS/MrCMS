using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IInPageAdminService
    {
        SaveResult SaveBodyContent(UpdatePropertyData updatePropertyData);
        string GetUnformattedBodyContent(GetPropertyData getPropertyData);
        string GetFormattedBodyContent(GetPropertyData getPropertyData, Controller controller);
    }
}