using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IProcessWebpageViews
    {
        void Process(ViewResult result, Webpage webpage);
    }
}