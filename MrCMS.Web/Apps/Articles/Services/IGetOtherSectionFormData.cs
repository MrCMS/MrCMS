using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IGetOtherSectionFormData
    {
        List<string> GetFormKeys(ControllerContext context);
        List<int> GetSectionIds(ControllerContext context);
    }
}