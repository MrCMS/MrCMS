using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetLayoutOptions
    {
        Task<List<SelectListItem>> Get();
    }
}