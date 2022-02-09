using System.Threading.Tasks;
using MrCMS.Web.Apps.Core.Models.Navigation;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public interface IGetNavigationRecords
    {
        Task<NavigationList> GetRootNavigation(bool includeChildren);
    }
}