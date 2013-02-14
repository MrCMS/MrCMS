using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFormService
    {
        string GetFormStructure(Webpage webpage);
        void SaveFormStructure(Webpage webpage, string data);
        void SaveFormData(Webpage webpage, FormCollection formCollection);
        PostingsModel GetFormPostings(Webpage webpage, int page, string search);
    }
}