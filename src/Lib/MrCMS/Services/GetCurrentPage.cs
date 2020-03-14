using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class GetCurrentPage : IGetCurrentPage, ISetCurrentPage
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public GetCurrentPage(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Webpage GetPage()
        {
            return _contextAccessor.HttpContext.Items[CurrentPage] as Webpage;
        }

        private const string CurrentPage = "current-page";
        public void SetPage(Webpage page)
        {
            _contextAccessor.HttpContext.Items[CurrentPage] = page;
        }
    }
}