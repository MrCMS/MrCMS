using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models.Navigation;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public interface IGetPageSubNavigation
    {
        Task<List<NavigationRecord>> GetChildNavigationRecords(Webpage page);
    }
}