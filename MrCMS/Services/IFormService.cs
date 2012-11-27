using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IFormService
    {
        string GetFormStructure(int id);
        void SaveFormStructure(int id, string data);
        void SaveFormData(int id, FormCollection formCollection);
        FormPosting GetFormPosting(int id);
    }
}