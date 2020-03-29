using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IFormListOptionAdminService
    {
        Task Add(AddFormListOptionModel formListOption);
        Task Update(UpdateFormListOptionModel formListOption);
        Task Delete(int formListOption);
        AddFormListOptionModel GetAddModel(int formPropertyId);
        Task<UpdateFormListOptionModel> GetUpdateModel(int id);
    }
}