using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFormService
    {
        string GetFormStructure(Webpage webpage);
        void SaveFormStructure(Webpage webpage, string data);
        List<string> SaveFormData(Webpage webpage, HttpRequestBase request);
        PostingsModel GetFormPostings(Webpage webpage, int page, string search);
        void AddFormProperty(FormProperty formProperty);
        void SaveFormProperty(FormProperty property);
        void DeleteFormProperty(FormProperty property);
        void SaveFormListOption(FormListOption formListOption);
        void UpdateFormListOption(FormListOption formListOption);
        void DeleteFormListOption(FormListOption formListOption);
    }
}