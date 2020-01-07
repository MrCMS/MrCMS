using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFormAdminService
    {
        IPagedList<Form> Search(FormSearchModel model);
        Task<Form> AddForm(AddFormModel model);
        Form GetForm(int id);
        UpdateFormModel GetUpdateModel(int id);
        Task Update(UpdateFormModel model);
        Task Delete(int id);


        PostingsModel GetFormPostings(Form form, int page, string search);
        Task SetOrders(List<SortItem> items);
        Task ClearFormData(Form form);
        byte[] ExportFormData(Form form);
        Task<FormPosting> DeletePosting(int id);
    }
}