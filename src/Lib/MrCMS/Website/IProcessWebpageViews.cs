using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IProcessWebpageViews
    {
        Task Process(ViewResult result, Webpage webpage);
    }
}