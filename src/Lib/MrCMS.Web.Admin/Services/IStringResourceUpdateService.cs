using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IStringResourceUpdateService
    {
        Task<FileResult> Export(StringResourceSearchQuery searchQuery);
        Task<ResourceImportSummary> Import(IFormFile file);
    }
}