using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IStringResourceUpdateService
    {
        FileResult Export(StringResourceSearchQuery searchQuery);
        ResourceImportSummary Import(IFormFile file);
    }
}