using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

public interface IContentRowAdminService
{
    Task<IReadOnlyList<ContentRowOption>> GetContentRowOptions();
}