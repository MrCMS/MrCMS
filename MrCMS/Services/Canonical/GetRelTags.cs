using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;

namespace MrCMS.Services.Canonical
{
    public abstract class GetRelTags
    {
        public abstract string GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        public abstract string GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }

    public abstract class GetRelTags<T> : GetRelTags where T : Webpage
    {
        public sealed override string GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            webpage = webpage.Unproxy();
            T typed = webpage as T;
            return typed == null ? null : GetPrev(typed, metadata, viewData);
        }
        public sealed override string GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            webpage = webpage.Unproxy();
            T typed = webpage as T;
            return typed == null ? null : GetNext(typed, metadata, viewData);
        }

        public abstract string GetPrev(T webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
        public abstract string GetNext(T webpage, PagedListMetaData metadata, ViewDataDictionary viewData);
    }
}