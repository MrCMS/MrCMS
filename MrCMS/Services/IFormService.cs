using System.Collections.Generic;
using System.Web;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFormService
    {
        List<string> SaveFormData(Webpage webpage, HttpRequestBase request);
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
        void DeletePosting(FormPosting posting);
    }
}