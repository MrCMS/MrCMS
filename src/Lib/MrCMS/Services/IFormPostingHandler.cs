using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IFormPostingHandler
    {
        Form GetForm(int id);
        List<string> SaveFormData(Form form, HttpRequest request);
        string GetRefererUrl();
    }
}