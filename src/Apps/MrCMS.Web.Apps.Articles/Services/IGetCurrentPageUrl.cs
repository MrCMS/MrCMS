using System.Threading.Tasks;

//Todo: potentially move this in core if best way
namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IGetCurrentPageUrl
    {
        Task<string> GetUrl(object queryString = null);
    }
}
