using System.Web;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface ISaveFormFileUpload
    {
        string SaveFile(Webpage webpage, FormPosting formPosting, HttpPostedFileBase file);
    }
}