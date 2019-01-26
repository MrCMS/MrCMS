using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface ISaveFormFileUpload
    {
        string SaveFile(Form form, FormPosting formPosting, IFormFile file);
    }
}