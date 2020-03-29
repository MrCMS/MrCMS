using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public interface ITemporaryPublisher
    {
        Task<IAsyncDisposable> TemporarilyPublish(Webpage webpage);
    }
}