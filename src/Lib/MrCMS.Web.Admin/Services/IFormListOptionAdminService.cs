using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IFormListOptionAdminService
    {
        void Add(AddFormListOptionModel formListOption);
        void Update(UpdateFormListOptionModel formListOption);
        void Delete(int formListOption);
        AddFormListOptionModel GetAddModel(int formPropertyId);
        UpdateFormListOptionModel GetUpdateModel(int id);
    }
}