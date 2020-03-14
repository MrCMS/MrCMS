using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IGetWebpageCachingInfo
    {
        Task<CachingInfo> Get(Webpage webpage, IQueryCollection queryData);
    }
}