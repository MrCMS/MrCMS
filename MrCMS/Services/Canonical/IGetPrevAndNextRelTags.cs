using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;

namespace MrCMS.Services.Canonical
{
    public interface IGetPrevAndNextRelTags
    {
        string GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        string GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }
}