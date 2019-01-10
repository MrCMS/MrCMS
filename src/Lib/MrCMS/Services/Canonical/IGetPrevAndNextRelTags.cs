using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;
using X.PagedList;

namespace MrCMS.Services.Canonical
{
    public interface IGetPrevAndNextRelTags
    {
        string GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        string GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }
}