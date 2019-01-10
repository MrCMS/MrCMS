using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IStringResourceUpdateService
    {
        FileResult Export(StringResourceSearchQuery searchQuery);
        ResourceImportSummary Import(IFormFile file);
    }
}