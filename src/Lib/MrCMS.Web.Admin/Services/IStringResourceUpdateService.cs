using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IStringResourceUpdateService
    {
        FileResult Export(StringResourceSearchQuery searchQuery);
        ResourceImportSummary Import(IFormFile file);
    }
}