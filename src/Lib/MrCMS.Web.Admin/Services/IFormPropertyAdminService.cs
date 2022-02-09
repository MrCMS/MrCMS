using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
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