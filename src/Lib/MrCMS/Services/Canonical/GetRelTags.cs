using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using X.PagedList;

namespace MrCMS.Services.Canonical
{
    public abstract class GetRelTags
    {
        public abstract Task<string> GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        public abstract Task<string> GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }

    public abstract class GetRelTags<T> : GetRelTags where T : Webpage
    {
        public sealed override async Task<string> GetPrev(Webpage webpage, PagedListMetaData metadata,
            ViewDataDictionary viewData)
        {
            webpage = webpage.Unproxy();
            return (webpage is not T typed) ? null : await GetPrev(typed, metadata, viewData);
        }

        public sealed override async Task<string> GetNext(Webpage webpage, PagedListMetaData metadata,
            ViewDataDictionary viewData)
        {
            webpage = webpage.Unproxy();
            return (webpage is not T typed) ? null : await GetNext(typed, metadata, viewData);
        }

        public abstract Task<string> GetPrev(T webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        public abstract Task<string> GetNext(T webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }
}