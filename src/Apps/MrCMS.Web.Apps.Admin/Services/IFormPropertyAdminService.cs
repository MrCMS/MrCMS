using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFormPropertyAdminService
    {
        void Add(AddFormPropertyModel model);
        void Update(UpdateFormPropertyModel property);
        void Delete(int id);
        List<SelectListItem> GetPropertyTypeOptions();
        UpdateFormPropertyModel GetUpdateModel(int id);
    }
}