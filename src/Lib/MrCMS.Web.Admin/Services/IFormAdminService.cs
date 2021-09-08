using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Forms;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IFormAdminService
    {
        Task<IPagedList<Form>> Search(FormSearchModel model);
        Task<Form> AddForm(AddFormModel model);
        Task<Form> GetForm(int id);
        Task<UpdateFormModel> GetUpdateModel(int id);
        Task Update(UpdateFormModel model);
        Task Delete(int id);
        Task<IPagedList<Webpage>> GetPagesWithForms(WebpagesWithEmbeddedFormQuery query);


        Task<PostingsModel> GetFormPostings(Form form, int page, string search);
        Task SetOrders(List<SortItem> items);
        Task ClearFormData(Form form);
        Task<byte[]> ExportFormData(Form form);
        Task<FormPosting> GetFormPosting(int id);
        Task<FormPosting> DeletePosting(int id);

        Task<WebpageViewModel> GetPostingCountForWebpage(int webpageId);
    }

    public class WebpageViewModel
    {
        public int Count { get; set; }
        public int? FormId { get; set; }
    }
}