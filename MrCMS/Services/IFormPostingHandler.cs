using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IFormPostingHandler
    {
        Webpage GetWebpage(int id);
        List<string> SaveFormData(Webpage webpage, HttpRequest request);
        string GetRefererUrl();
    }
}