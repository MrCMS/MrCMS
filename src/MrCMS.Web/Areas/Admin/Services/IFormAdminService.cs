using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IFormAdminService
    {
        IPagedList<Form> Search(FormSearchModel model);
        Task<Form> AddForm(AddFormModel model);
        Form GetForm(int id);
        UpdateFormModel GetUpdateModel(int id);
        Task<List<FormProperty>> GetProperties(int formId);
        Task<List<SortItem>> GetSortItems(int formId);
        Task Update(UpdateFormModel model);
        Task Delete(int id);


        PostingsModel GetFormPostings(Form form, int page, string search);
        Task SetOrders(List<SortItem> items);
        Task ClearFormData(Form form);
        Task<byte[]> ExportFormData(Form form);
        Task<FormPosting> DeletePosting(int id);
    }
}