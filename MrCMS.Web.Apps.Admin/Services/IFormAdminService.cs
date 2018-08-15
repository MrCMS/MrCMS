using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFormAdminService
    {
        PostingsModel GetFormPostings(Webpage webpage, int page, string search);
        void AddFormProperty(FormProperty formProperty);
        void SaveFormProperty(FormProperty property);
        void DeleteFormProperty(FormProperty property);
        void SaveFormListOption(FormListOption formListOption);
        void UpdateFormListOption(FormListOption formListOption);
        void DeleteFormListOption(FormListOption formListOption);
        void SetOrders(List<SortItem> items);
        void ClearFormData(Webpage webpage);
        byte[] ExportFormData(Webpage webpage);
        FormPosting DeletePosting(int id);
    }
}