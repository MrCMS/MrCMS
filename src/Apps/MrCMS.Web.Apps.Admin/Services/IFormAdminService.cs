using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFormAdminService
    {
        IPagedList<Form> Search(FormSearchModel model);
        Form AddForm(AddFormModel model);
        Form GetForm(int id);
        UpdateFormModel GetUpdateModel(int id);
        void Update(UpdateFormModel model);
        void Delete(int id);


        PostingsModel GetFormPostings(Form form, int page, string search);
        void SetOrders(List<SortItem> items);
        void ClearFormData(Form form);
        byte[] ExportFormData(Form form);
        FormPosting DeletePosting(int id);
    }
}