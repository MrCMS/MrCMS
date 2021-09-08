using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface ISaveFormFileUpload
    {
        Task<string> SaveFile(Form form, FormPosting formPosting, IFormFile file);
    }
}