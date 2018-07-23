using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface ISaveFormFileUpload
    {
        string SaveFile(Webpage webpage, FormPosting formPosting, IFormFile file);
    }
}