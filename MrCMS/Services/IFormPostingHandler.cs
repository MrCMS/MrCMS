using System.Collections.Generic;
using System.Web;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IFormPostingHandler
    {
        List<string> SaveFormData(Webpage webpage, HttpRequestBase request);
    }
}