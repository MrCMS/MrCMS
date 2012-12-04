using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFormService
    {
        string GetFormStructure(Webpage webpage);
        void SaveFormStructure(int id, string data);
        void SaveFormData(int id, FormCollection formCollection);
        FormPosting GetFormPosting(int id);
        PostingsModel GetFormPostings(Webpage webpage, int page, string search);
    }
}