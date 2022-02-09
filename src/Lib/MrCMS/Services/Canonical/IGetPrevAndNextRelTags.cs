using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;
using X.PagedList;

namespace MrCMS.Services.Canonical
{
    public interface IGetPrevAndNextRelTags
    {
        Task<string> GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        Task<string> GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }
}