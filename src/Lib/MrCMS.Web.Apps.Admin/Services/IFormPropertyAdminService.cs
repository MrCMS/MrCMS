using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFormPropertyAdminService
    {
        Task Add(AddFormPropertyModel model);
        Task Update(UpdateFormPropertyModel property);
        Task Delete(int id);
        List<SelectListItem> GetPropertyTypeOptions();
        Task<UpdateFormPropertyModel> GetUpdateModel(int id);
    }
}